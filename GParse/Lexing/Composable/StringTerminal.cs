using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a node that matches a sequence of characters
    /// </summary>
    public sealed class StringTerminal : GrammarNode<Char>
    {
        /// <summary>
        /// The string of characters this node matches
        /// </summary>
        public String String { get; }

        /// <summary>
        /// Initializes this string node
        /// </summary>
        /// <param name="str"></param>
        public StringTerminal ( String str )
        {
            this.String = str;
        }

        /// <summary>
        /// The implicit conversion from strings to string nodes
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator StringTerminal ( String str ) =>
            new StringTerminal ( str );
    }
}
