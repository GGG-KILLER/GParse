using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using GParse.Lexing;
using Tsu.Expressions;
using Tsu.StateMachines.Transducers;

namespace GParse.Extensions.StateMachines
{
    /// <summary>
    /// Represents a compiled <see cref="Transducer{InputT, OutputT}" /> that accepts a
    /// <see cref="ITokenReader{TTokenType}" /> as an input provider
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="OutputT"></typeparam>
    /// <param name="reader"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public delegate Boolean TokenReaderTransducer<TTokenType, OutputT>(ITokenReader<TTokenType> reader, [AllowNull] out OutputT output)
        where TTokenType : notnull;

    /// <summary>
    /// Extensions for a <see cref="Transducer{InputT, OutputT}" /> to work with a
    /// <see cref="TokenReader{TTokenType}" />
    /// </summary>
    public static class ITokenReaderTransducerExtensions
    {
        /// <summary>
        /// Attempts to execute the state machine against the <see cref="TokenReader{TTokenType}" />
        /// </summary>
        /// <typeparam name="TTokenType"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <param name="reader"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Boolean TryExecute<TTokenType, OutputT>(this Transducer<Token<TTokenType>, OutputT> transducer, ITokenReader<TTokenType> reader, [MaybeNull] out OutputT output)
            where TTokenType : notnull
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var offset = 0;
            TransducerState<Token<TTokenType>, OutputT> state = transducer.InitialState;
            while (reader.Position < reader.Length)
            {
                if (!state.TransitionTable.TryGetValue(reader.Lookahead(offset), out TransducerState<Token<TTokenType>, OutputT>? tmp))
                    break;
                state = tmp;
                offset++;
            }

            if (state.IsTerminal)
            {
                reader.Skip(offset + 1);
                output = state.Output;
                return true;
            }

            output = default;
            return false;
        }

        private static SwitchExpression CompileState<TTokenType, OutputT>(TransducerState<Token<TTokenType>, OutputT> state, ParameterExpression reader, ParameterExpression output, LabelTarget @return, Int32 depth)
            where TTokenType : notnull
        {
            var idx = 0;
            var cases = new SwitchCase[state.TransitionTable.Count];
            foreach (KeyValuePair<Token<TTokenType>, TransducerState<Token<TTokenType>, OutputT>> statePair in state.TransitionTable)
            {
                cases[idx++] = Expression.SwitchCase(
                    CompileState(statePair.Value, reader, output, @return, depth + 1),
                    Expression.Constant(statePair.Key)
                );
            }

            return Expression.Switch(
                GExpression.MethodCall<ITokenReader<TTokenType>>(reader, r => r.Lookahead(depth), depth),
                state.IsTerminal
                    ? Expression.Block(
                        GExpression.MethodCall<ITokenReader<TTokenType>>(reader, r => r.Skip(0), depth + 1),
                        Expression.Assign(output, Expression.Constant(state.Output)),
                        Expression.Return(@return, Expression.Constant(true))
                    )
                    : (Expression) Expression.Constant(false),
                cases
            );
        }

        /// <summary>
        /// Compiles this <see cref="Transducer{InputT, OutputT}" />
        /// </summary>
        /// <typeparam name="TTokenType"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <returns></returns>
        public static TokenReaderTransducer<TTokenType, OutputT> CompileWithTokenReaderAsInput<TTokenType, OutputT>(this Transducer<Token<TTokenType>, OutputT> transducer)
            where TTokenType : notnull
        {
            ParameterExpression reader = Expression.Parameter(typeof(ITokenReader<TTokenType>), "reader");
            ParameterExpression output = Expression.Parameter(typeof(OutputT).MakeByRefType(), "output");
            LabelTarget @return = Expression.Label(typeof(Boolean));

            return Expression.Lambda<TokenReaderTransducer<TTokenType, OutputT>>(
                Expression.Block(
                    CompileState(transducer.InitialState, reader, output, @return, 0),
                    Expression.Label(@return, Expression.Constant(false))
                ),
                reader,
                output
            ).Compile();
        }
    }
}