using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common.IO;
using GParse.Common.Utilities;

namespace GParse.StateMachines
{
    /// <summary>
    /// Serializes a transducer into another format
    /// </summary>
    public static class TransducerSerializer
    {
        /// <summary>
        /// Serializes a transducer into C# code
        /// </summary>
        /// <typeparam name="InputT">
        /// The type of input the transducer accepts
        /// </typeparam>
        /// <typeparam name="OutputT">
        /// The type of output the transducer outputs
        /// </typeparam>
        /// <param name="name">
        /// The name of the variable the transducer will be
        /// assigned to
        /// </param>
        /// <param name="transducer">The transducer to serialize</param>
        /// <param name="inputSerializer">The input value serializer</param>
        /// <param name="outputSerializer">
        /// The output value serializer
        /// </param>
        /// <returns>The string with the serialized C# code</returns>
        public static String GetTransducerConstructorCode<InputT, OutputT> ( String name, Transducer<InputT, OutputT> transducer, Func<InputT, String> inputSerializer, Func<OutputT, String> outputSerializer )
        {
            if ( inputSerializer == null )
                throw new ArgumentNullException ( nameof ( inputSerializer ) );
            if ( outputSerializer == null )
                throw new ArgumentNullException ( nameof ( outputSerializer ) );

            var writer = new CodeWriter ( );
            if ( transducer.InitialState.IsTerminal )
                writer.WriteLineIndented ( $"var {name} = new Transducer<{typeof ( InputT ).Name}, {typeof ( OutputT ).Name}> ( {outputSerializer ( transducer.InitialState.Output )} );" );
            else
                writer.WriteLineIndented ( $"var {name} = new Transducer<{typeof ( InputT ).Name}, {typeof ( OutputT ).Name}> ( );" );

            if ( transducer.InitialState.TransitionTable.Count > 0 )
            {
                foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in transducer.InitialState.TransitionTable )
                {
                    if ( kv.Value.TransitionTable.Count > 0 )
                    {
                        if ( kv.Value.IsTerminal )
                        {
                            writer.WriteLineIndented ( $"{name}.InitialState.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( kv.Value.Output )}, state0 =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => GetStateConstructorCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                        else
                        {
                            writer.WriteLineIndented ( $"{name}.InitialState.OnInput ( {inputSerializer ( kv.Key )}, state0 =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => GetStateConstructorCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                    }
                    else if ( kv.Value.IsTerminal )
                        writer.WriteLineIndented ( $"{name}.InitialState.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( kv.Value.Output )} );" );
                    else
                        throw new InvalidOperationException ( "A state without any transitions nor output" );
                }
            }

            return writer.GetCode ( );
        }

        private static void GetStateConstructorCode<InputT, OutputT> ( CodeWriter writer, TransducerState<InputT, OutputT> state, Func<InputT, String> inputSerializer, Func<OutputT, String> outputSerializer )
        {
            if ( inputSerializer == null )
                throw new ArgumentNullException ( nameof ( inputSerializer ) );
            if ( outputSerializer == null )
                throw new ArgumentNullException ( nameof ( outputSerializer ) );

            if ( state.TransitionTable.Count > 0 )
            {
                foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in state.TransitionTable )
                {
                    if ( kv.Value.TransitionTable.Count > 0 )
                    {
                        if ( state.IsTerminal )
                        {
                            writer.WriteLineIndented ( $"state{writer.Indentation - 1}.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( state.Output )}, state{writer.Indentation} =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => GetStateConstructorCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                        else
                        {
                            writer.WriteLineIndented ( $"state{writer.Indentation - 1}.OnInput ( {inputSerializer ( kv.Key )}, state{writer.Indentation} =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => GetStateConstructorCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                    }
                    else if ( kv.Value.IsTerminal )
                    {
                        writer.WriteLineIndented ( $"state{writer.Indentation - 1}.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( kv.Value.Output )} );" );
                    }
                    else
                        throw new InvalidOperationException ( "A state without any transitions nor output" );
                }
            }
        }

        /// <summary>
        /// Transforms the transducer into an expression tree
        /// </summary>
        /// <typeparam name="InputT">
        /// The type of input accepted by the transducer
        /// </typeparam>
        /// <typeparam name="OutputT">
        /// The type of output emitted by the transducer
        /// </typeparam>
        /// <param name="transducer">The transducer to compile</param>
        /// <returns></returns>
        public static Expression<Func<IEnumerable<InputT>, (Int32, OutputT)>> GetExpressionTree<InputT, OutputT> ( Transducer<InputT, OutputT> transducer )
        {
            ParameterExpression inputArg = Expression.Parameter ( typeof ( IEnumerable<InputT> ), "input" );
            ParameterExpression enumerator = Expression.Variable ( typeof ( IEnumerator<InputT> ), "enumerator" );
            LabelTarget returnLabelTarget = Expression.Label ( typeof ( (Int32, OutputT) ), "return-label" );

            var lambda = Expression.Lambda<Func<IEnumerable<InputT>, (Int32, OutputT)>> (
                Expression.Block (
                    typeof ( (Int32, OutputT) ),
                    new[] { enumerator },
                    Expression.Assign ( enumerator, ExprUtils.MethodCall<IEnumerable<InputT>> ( inputArg, "GetEnumerator" ) ),
                    Expression.TryFinally (
                        GetStateExpressionTree ( transducer.InitialState, enumerator, 0, returnLabelTarget ),
                        Expression.IfThen (
                            Expression.NotEqual ( enumerator, Expression.Constant ( null ) ),
                            ExprUtils.MethodCall<IDisposable> ( enumerator, "Dispose" ) )
                    )
                ),
                inputArg
            );
            return lambda;
        }

        private static Expression GetStateExpressionTree<InputT, OutputT> ( TransducerState<InputT, OutputT> state, ParameterExpression enumerator, Int32 depth, LabelTarget returnLabelTarget )
        {
            if ( state.TransitionTable.Count > 0 )
            {
                // Serialize all transitions
                var cases = new List<SwitchCase> ( );
                foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in state.TransitionTable )
                    cases.Add ( Expression.SwitchCase (
                        GetStateExpressionTree ( kv.Value, enumerator, depth + 1, returnLabelTarget ),
                        Expression.Constant ( kv.Key )
                    ) );

                ConstantExpression retVal = state.IsTerminal
                    ? Expression.Constant ( default ( (Int32, OutputT) ) )
                    : Expression.Constant ( ( depth, state.Output ) );

                return Expression.Condition (
                    ExprUtils.MethodCall<IEnumerator> ( enumerator, "MoveNext" ),
                    Expression.Switch (
                        typeof ( (Int32, OutputT) ),
                        ExprUtils.PropertyGet<IEnumerator<InputT>> ( enumerator, "Current" ),
                        retVal,
                        null,
                        cases.ToArray ( ) ),
                    // Then serialize the output if this is a
                    // terminal state, otherwise return the error values
                    retVal
                );
            }
            else if ( state.IsTerminal )
            {
                return Expression.Constant ( (depth, state.Output) );
            }

            throw new InvalidOperationException ( "Cannot serialize a non-terminal state without any transitions." );
        }

        /// <summary>
        /// Compiles a transducer into a method for faster execution
        /// </summary>
        /// <typeparam name="InputT"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <returns></returns>
        public static Func<IEnumerable<InputT>, (Int32, OutputT)> Compile<InputT, OutputT> ( Transducer<InputT, OutputT> transducer )
        {
            Expression<Func<IEnumerable<InputT>, (Int32, OutputT)>> lambda = GetExpressionTree ( transducer );
            Func<IEnumerable<InputT>, (Int32, OutputT)> compiled = lambda.Compile ( );
            return compiled;
        }
    }
}
