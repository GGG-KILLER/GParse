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

        public Boolean TryGetOutput ( IEnumerator<InputT> inputEnumerator, out OutputT output )
        {
            State<InputT, OutputT> state = this.InitialState;

            while ( inputEnumerator.MoveNext ( ) )
            {
                if ( state.TransitionTable.TryGetValue ( inputEnumerator.Current, out State<InputT, OutputT> tmpState ) )
                {
                    if ( tmpState.IsTerminal )
                    {
                        output = tmpState.Output;
                        return true;
                    }
                    else
                        state = tmpState;
                }
                else
                    break;
            }

            output = default;
            return false;
        }
    }
}
