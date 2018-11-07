using System;
using System.Collections.Generic;

namespace GParse.StateMachines
{
    public class Transducer<InputT, OutputT> : ICloneable
    {
        public readonly State<InputT, OutputT> InitialState;

        public Transducer ( )
        {
            this.InitialState = new State<InputT, OutputT> ( );
        }

        public Transducer ( OutputT output )
        {
            this.InitialState = new State<InputT, OutputT> ( output );
        }

        public Transducer<InputT, OutputT> WithDefaultOutput ( OutputT output )
        {
            var transducer = new Transducer<InputT,OutputT> ( output );
            foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in this.InitialState.TransitionTable )
                transducer.InitialState.TransitionTable[kv.Key] = ( State<InputT, OutputT> ) kv.Value.Clone ( );
            return transducer;
        }

        public Transducer<InputT, OutputT> WithoutDefaultOutput ( )
        {
            var transducer = new Transducer<InputT,OutputT> ( );
            foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in this.InitialState.TransitionTable )
                transducer.InitialState.TransitionTable[kv.Key] = ( State<InputT, OutputT> ) kv.Value.Clone ( );
            return transducer;
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
            if ( @string == null )
                throw new ArgumentNullException ( nameof ( @string ) );

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

            if ( state.IsTerminal )
            {
                output = state.Output;
                return consumedInputs;
            }

            output = default;
            return 0;
        }
    }
}
