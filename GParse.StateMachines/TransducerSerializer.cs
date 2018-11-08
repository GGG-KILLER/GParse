using System;
using System.Collections.Generic;
using GParse.Common.IO;

namespace GParse.StateMachines
{
    public static class TransducerSerializer
    {
        public static String SerializeAsCode<InputT, OutputT> ( String name, Transducer<InputT, OutputT> transducer, Func<InputT, String> inputSerializer, Func<OutputT, String> outputSerializer )
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
                foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in transducer.InitialState.TransitionTable )
                {
                    if ( kv.Value.TransitionTable.Count > 0 )
                    {
                        if ( kv.Value.IsTerminal )
                        {
                            writer.WriteLineIndented ( $"{name}.InitialState.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( kv.Value.Output )}, state0 =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => SerializeStateAsCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                        else
                        {
                            writer.WriteLineIndented ( $"{name}.InitialState.OnInput ( {inputSerializer ( kv.Key )}, state0 =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => SerializeStateAsCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
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

        private static void SerializeStateAsCode<InputT, OutputT> ( CodeWriter writer, State<InputT, OutputT> state, Func<InputT, String> inputSerializer, Func<OutputT, String> outputSerializer )
        {
            if ( inputSerializer == null )
                throw new ArgumentNullException ( nameof ( inputSerializer ) );
            if ( outputSerializer == null )
                throw new ArgumentNullException ( nameof ( outputSerializer ) );

            if ( state.TransitionTable.Count > 0 )
            {
                foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in state.TransitionTable )
                {
                    if ( kv.Value.TransitionTable.Count > 0 )
                    {
                        if ( state.IsTerminal )
                        {
                            writer.WriteLineIndented ( $"state{writer.Indentation - 1}.OnInput ( {inputSerializer ( kv.Key )}, {outputSerializer ( state.Output )}, state{writer.Indentation} =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => SerializeStateAsCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
                            writer.WriteLineIndented ( "} );" );
                        }
                        else
                        {
                            writer.WriteLineIndented ( $"state{writer.Indentation - 1}.OnInput ( {inputSerializer ( kv.Key )}, state{writer.Indentation} =>" );
                            writer.WriteLineIndented ( "{" );
                            writer.WithIndentation ( ( ) => SerializeStateAsCode ( writer, kv.Value, inputSerializer, outputSerializer ) );
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
    }
}
