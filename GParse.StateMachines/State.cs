using System;
using System.Collections.Generic;

namespace GParse.StateMachines
{
    public class State<InputT, OutputT> : ICloneable
    {
        public readonly Boolean IsTerminal;
        public readonly OutputT Output;
        public readonly Dictionary<InputT, State<InputT, OutputT>> TransitionTable = new Dictionary<InputT, State<InputT, OutputT>> ( );

        public State ( )
        {
            this.IsTerminal = false;
        }

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
                throw new InvalidOperationException ( "A transition for this input already exists." );
            return this.TransitionTable[input] = new State<InputT, OutputT> ( output );
        }

        public State<InputT, OutputT> OnInput ( InputT input, Action<State<InputT, OutputT>> action )
        {
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );
            if ( this.IsTerminal )
                throw new InvalidOperationException ( "Cannot have a transition in a terminal state." );

            action ( this.GetState ( input ) );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT input, OutputT output )
        {
            if ( this.IsTerminal )
                throw new InvalidOperationException ( "Cannot have a transition in a terminal state." );
            this.GetState ( input, output );
            return this;
        }

        public State<InputT, OutputT> OnInput ( InputT[] @string, Action<State<InputT, OutputT>> action, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );
            if ( this.IsTerminal )
                throw new InvalidOperationException ( "Cannot have a transition in a terminal state." );

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
            if ( this.IsTerminal )
                throw new InvalidOperationException ( "Cannot have a transition in a terminal state." );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, output, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], output );

            return this;
        }

        public Object Clone ( )
        {
            State<InputT, OutputT> state = this.IsTerminal ? new State<InputT, OutputT> ( this.Output ) : new State<InputT, OutputT> ( );
            foreach ( KeyValuePair<InputT, State<InputT, OutputT>> kv in this.TransitionTable )
                state.TransitionTable[kv.Key] = ( State<InputT, OutputT> ) kv.Value.Clone ( );
            return state;
        }
    }
}
