using System;
using System.Collections.Generic;

namespace GParse.StateMachines
{
    public class Transducer<InputT, OutputT> : ICloneable
    {
        public readonly State<InputT, OutputT> InitialState = new State<InputT, OutputT> ( );

        public Transducer ( )
        {
        }

        #region ICloneable

        private Transducer ( Transducer<InputT, OutputT> transducer )
        {
            this.InitialState = ( State<InputT, OutputT> ) transducer.InitialState.Clone ( );
        }

        public Object Clone ( ) => new Transducer<InputT, OutputT> ( this );

        #endregion ICloneable

        public Int32 Execute ( IEnumerable<InputT> @string, out OutputT output )
        {
            State<InputT, OutputT> state = this.InitialState;
            var consumedInputs = 0;

            foreach ( InputT value in @string )
            {
                if ( state.TransitionTable.TryGetValue ( value, out State<InputT, OutputT> nextState ) )
                {
                    state = nextState;
                }
                else if ( state.IsTerminal )
                {
                    output = state.Output;
                    return consumedInputs;
                }
                else
                {
                    // Quit the loop on a non-terminal state with
                    // no available transitions
                    break;
                }

                consumedInputs++;
            }

            output = default;
            return 0;
        }
    }
}
