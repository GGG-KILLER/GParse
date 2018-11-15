using System;
using System.Collections.Generic;

namespace GParse.StateMachines
{
    /// <summary>
    /// Represents a state of the <see cref="Transducer{InputT, OutputT}" />
    /// </summary>
    /// <typeparam name="InputT">
    /// The type of input the
    /// <see cref="Transducer{InputT, OutputT}" /> accepts
    /// </typeparam>
    /// <typeparam name="OutputT">
    /// The type of the output the
    /// <see cref="Transducer{InputT, OutputT}" /> emits
    /// </typeparam>
    public class TransducerState<InputT, OutputT>
    {
        /// <summary>
        /// Whether this is a terminal state (one that has an
        /// output related with it)
        /// </summary>
        public readonly Boolean IsTerminal;

        /// <summary>
        /// The output of the state if it is a terminal state
        /// </summary>
        public readonly OutputT Output;

        /// <summary>
        /// The transitions this input has
        /// </summary>
        public readonly Dictionary<InputT, TransducerState<InputT, OutputT>> TransitionTable = new Dictionary<InputT, TransducerState<InputT, OutputT>> ( );

        /// <summary>
        /// Creates a new non-terminal state
        /// </summary>
        public TransducerState ( )
        {
            this.IsTerminal = false;
        }

        /// <summary>
        /// Creates a new terminal state with the ouput
        /// </summary>
        /// <param name="output"></param>
        public TransducerState ( OutputT output )
        {
            this.IsTerminal = true;
            this.Output = output;
        }

        private TransducerState<InputT, OutputT> GetState ( InputT input ) =>
            this.TransitionTable.ContainsKey ( input )
                ? this.TransitionTable[input]
                : this.TransitionTable[input] = new TransducerState<InputT, OutputT> ( );

        private TransducerState<InputT, OutputT> GetState ( InputT input, OutputT output )
        {
            if ( this.TransitionTable.ContainsKey ( input ) )
            {
                TransducerState<InputT, OutputT> state = this.TransitionTable[input];
                // Create a new state with all transitions of the
                // previous state
                var newState = new TransducerState<InputT, OutputT> ( output );
                foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in state.TransitionTable )
                    newState.TransitionTable[kv.Key] = kv.Value;

                return this.TransitionTable[input] = newState;
            }
            else
                return this.TransitionTable[input] = new TransducerState<InputT, OutputT> ( output );
        }

        /// <summary>
        /// Adds a new transition to a new terminal state
        /// </summary>
        /// <param name="input">
        /// The input that will trigger the transition
        /// </param>
        /// <param name="output">
        /// The output that will be returned in this state
        /// </param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT input, OutputT output )
        {
            this.GetState ( input, output );
            return this;
        }

        /// <summary>
        /// Adds a new transition to a new non-terminal state
        /// </summary>
        /// <param name="input">
        /// The input that will trigger the transition
        /// </param>
        /// <param name="action">
        /// The action to configure the transitions of the new state
        /// </param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT input, Action<TransducerState<InputT, OutputT>> action )
        {
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );

            action ( this.GetState ( input ) );
            return this;
        }

        /// <summary>
        /// Adds a new transition to a new terminal state
        /// </summary>
        /// <param name="input">
        /// THe input that will trigger the transition
        /// </param>
        /// <param name="output">
        /// The output that will be returned on this state
        /// </param>
        /// <param name="action"></param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT input, OutputT output, Action<TransducerState<InputT, OutputT>> action )
        {
            if ( action == null )
                throw new ArgumentNullException ( nameof ( action ) );

            action ( this.GetState ( input, output ) );
            return this;
        }

        /// <summary>
        /// Adds a new transition to a non-terminal state
        /// </summary>
        /// <param name="string">
        /// The string of inputs that will trigger the transition
        /// </param>
        /// <param name="action">
        /// The action that will configure the transitions of the
        /// non-terminal state
        /// </param>
        /// <param name="startIndex">
        /// The index to start adding transitions from the <paramref name="string" />
        /// </param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT[] @string, Action<TransducerState<InputT, OutputT>> action, Int32 startIndex = 0 )
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

        /// <summary>
        /// Adds a new transition to a terminal state
        /// </summary>
        /// <param name="string">
        /// The string of inputs that will trigger the transition
        /// </param>
        /// <param name="output">
        /// The output of the terminal state
        /// </param>
        /// <param name="startIndex">
        /// The index to start adding transitions from the <paramref name="string" />
        /// </param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT[] @string, OutputT output, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, output, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], output );

            return this;
        }

        /// <summary>
        /// Adds a new transition to a terminal state
        /// </summary>
        /// <param name="string">
        /// The string of inputs that will trigger the transition
        /// </param>
        /// <param name="output">
        /// The output of the terminal state
        /// </param>
        /// <param name="action">
        /// The action that will configure the transitions of the
        /// terminal state
        /// </param>
        /// <param name="startIndex">
        /// The index to start adding transitions from the <paramref name="string" />
        /// </param>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> OnInput ( InputT[] @string, OutputT output, Action<TransducerState<InputT, OutputT>> action, Int32 startIndex = 0 )
        {
            if ( startIndex < 0 || startIndex >= @string.Length )
                throw new ArgumentOutOfRangeException ( nameof ( startIndex ), "Index was outside the bounds of the string." );

            if ( startIndex < @string.Length - 1 )
                this.OnInput ( @string[startIndex], state => state.OnInput ( @string, output, action, startIndex + 1 ) );
            else
                this.OnInput ( @string[startIndex], output, action );

            return this;
        }

        /// <summary>
        /// Creates a shallow copy of this state
        /// </summary>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> ShallowCopy ( )
        {
            TransducerState<InputT, OutputT> state = this.IsTerminal ? new TransducerState<InputT, OutputT> ( this.Output ) : new TransducerState<InputT, OutputT> ( );
            foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in this.TransitionTable )
                state.TransitionTable[kv.Key] = kv.Value;
            return state;
        }

        /// <summary>
        /// Creates a deep copy of this state
        /// </summary>
        /// <returns></returns>
        public TransducerState<InputT, OutputT> DeepCopy ( )
        {
            TransducerState<InputT, OutputT> state = this.IsTerminal ? new TransducerState<InputT, OutputT> ( this.Output ) : new TransducerState<InputT, OutputT> ( );
            foreach ( KeyValuePair<InputT, TransducerState<InputT, OutputT>> kv in this.TransitionTable )
                state.TransitionTable[kv.Key] = kv.Value.DeepCopy ( );
            return state;
        }
    }
}
