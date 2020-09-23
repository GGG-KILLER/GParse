using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a numbered capture group.
    /// </summary>
    public sealed class NumberedCapture : GrammarNodeContainer<Char>
    {
        /// <summary>
        /// The capture's position.
        /// </summary>
        public Int32 Position { get; }

        /// <summary>
        /// Initializes a new numbered capture group.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="node"></param>
        public NumberedCapture ( Int32 position, GrammarNode<Char> node ) : base ( node )
        {
            this.Position = position;
        }
    }
}
