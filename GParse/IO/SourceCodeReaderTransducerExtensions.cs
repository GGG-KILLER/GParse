using System;
using GParse.StateMachines;
using GParse.StateMachines.Transducers;

namespace GParse.IO
{
    /// <summary>
    /// Extensions to the <see cref="Transducer{InputT, OutputT}" /> class for operating on
    /// <see cref="SourceCodeReader" /> instances
    /// </summary>
    public static class SourceCodeReaderTransducerExtensions
    {
        /// <summary>
        /// Attempts to execute a transducer with content from a <see cref="SourceCodeReader" />. If the
        /// state reached is not a terminal state, the reader is rewinded to it's initial position.
        /// </summary>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <param name="reader"></param>
        /// <param name="output"></param>
        /// <returns>Whether the state reached was a terminal state.</returns>
        public static Boolean TryExecute<OutputT> ( this Transducer<Char, OutputT> transducer, SourceCodeReader reader, out OutputT output )
        {
            if ( reader == null )
                throw new ArgumentNullException ( nameof ( reader ) );

            SourceLocation startLocation = reader.Location;
            TransducerState<Char, OutputT> state = transducer.InitialState;
            while ( reader.HasContent )
            {
                if ( !state.TransitionTable.TryGetValue ( ( Char ) reader.Peek ( ), out TransducerState<Char, OutputT> tmp ) )
                    break;
                state = tmp;
                reader.Advance ( 1 );
            }

            if ( state.IsTerminal )
            {
                output = state.Output;
                return true;
            }

            reader.Rewind ( startLocation );
            output = default;
            return false;
        }
    }
}
