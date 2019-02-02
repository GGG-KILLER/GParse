using System;
using GParse.StateMachines.Transducers;

namespace GParse.Lexing
{
    /// <summary>
    /// Extensions for a <see cref="Transducer{InputT, OutputT}" /> to work with a
    /// <see cref="TokenReader{TokenTypeT}" />
    /// </summary>
    public static class TokenReaderTransducerExtensions
    {
        /// <summary>
        /// Attempts to execute the state machine against the <see cref="TokenReader{TokenTypeT}" />
        /// </summary>
        /// <typeparam name="TokenTypeT"></typeparam>
        /// <typeparam name="OutputT"></typeparam>
        /// <param name="transducer"></param>
        /// <param name="reader"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Boolean TryExecute<TokenTypeT, OutputT> ( this Transducer<Token<TokenTypeT>, OutputT> transducer, ITokenReader<TokenTypeT> reader, out OutputT output )
        {
            if ( reader == null )
                throw new ArgumentNullException ( nameof ( reader ) );

            var offset = 0;
            TransducerState<Token<TokenTypeT>, OutputT> state = transducer.InitialState;
            while ( !reader.IsAhead ( default ( TokenTypeT ) ) )
            {
                if ( !state.TransitionTable.TryGetValue ( reader.Lookahead ( offset ), out TransducerState<Token<TokenTypeT>, OutputT> tmp ) )
                    break;
                state = tmp;
                offset++;
            }

            if ( state.IsTerminal )
            {
                reader.Skip ( offset + 1 );
                output = state.Output;
                return true;
            }

            output = default;
            return false;
        }
    }
}
