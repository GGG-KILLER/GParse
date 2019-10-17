using System;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a terminal
    /// </summary>
    public class Terminal : GrammarNode
    {
        /// <summary>
        /// The value of the terminal
        /// </summary>
        public String Value { get; }

        /// <summary>
        /// Initializes a new terminal
        /// </summary>
        /// <param name="value"></param>
        public Terminal ( String value )
        {
            this.Value = value;
        }
    }
}
