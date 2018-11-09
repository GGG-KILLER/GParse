using System;
using System.Collections.Generic;

namespace GParse.StateMachines
{
    public class State<InputT, OutputT>
    {
        public readonly Boolean IsTerminal;
        public readonly OutputT Output;
        public readonly Dictionary<InputT, State<InputT, OutputT>> TransitionTable = new Dictionary<InputT, State<InputT, OutputT>> ( );

        public State ( )
        {
            this.IsTerminal = false;
        }

        /// <summary>
        /// Creates a terminal state with the ouput
        /// </summary>
        /// <param name="output"></param>
        public State ( OutputT output )
        {
            this.IsTerminal = true;
            this.Output = output;
        }

        private State<InputT, OutputT> GetState ( InputT input ) =>
            this.TransitionTable.ContainsKey ( input )
                ? this.TransitionTable[input]
                : this.TransitionTable[input] = new State<InputT, OutputT> ( );

        private State<InputT, OutputT> GetState ( InputT input, OutputT output )
        {
            if ( this.TransitionTable.ContainsKey ( input ) )
            {
                State<InputT, OutputT> state = this.TransitionTable[input];
                // Create a new state with all transitions of the
                // previous state
                var newState = new State<InputT, OutputT> ( output );
                foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in state.TransitionTable )
                    newState.TransitionTable[kv.Key] = kv.Value;

                return this.TransitionTable[input] = newState;
            }
            else
                return this.TransitionTable[input] = new State<InputT, OutputT> ( output );
        }

        public State<InputT, OutputT> OnInput ( InputT input, OutputT output )
        {
            this.GetState ( input, output );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT input, Action<State<InputT, OutputT>> action )
        {
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );

            action ( this.GetState ( input ) );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT input, OutputT output, Action<State<InputT, OutputT>> action )
        {
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );

            action ( this.GetState ( input, output ) );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT[] @string, Action<State<InputT, OutputT>> action, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, action, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], action );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT[] @string, OutputT output, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, output, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], output );

            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT[] @string, OutputT output, Action<State<InputT, OutputT>> action, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, output, action, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], output, action );

            return this;
        }

        public State<InputT, OutputT> ShallowCopy ( )
        {
            State<InputT, OutputT> state = this.IsTerminal ? new State<InputT, OutputT> ( this.Output ) : new State<InputT, OutputT> ( );
            foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in this.TransitionTable )
                state.TransitionTable[kv.Key] = kv.Value;
            return state;
        }

        public State<InputT, OutputT> DeepCopy ( )
        {
            State<InputT, OutputT> state = this.IsTerminal ? new State<InputT, OutputT> ( this.Output ) : new State<InputT, OutputT> ( );
            foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in this.TransitionTable )
                state.TransitionTable[kv.Key] = kv.Value.DeepCopy ( );
            return state;
        }
    }
}
