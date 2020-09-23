using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a lazy repetition.
    /// </summary>
    public sealed class Lazy : GrammarNodeContainer<Char>
    {
        /// <inheritdoc cref="GrammarNodeContainer{T}.InnerNode"/>
        public new Repetition<Char> InnerNode => ( Repetition<Char> ) base.InnerNode;

        /// <summary>
        /// Creates a new lazy repetition.
        /// </summary>
        /// <param name="node"></param>
        public Lazy ( Repetition<Char> node ) : base ( node )
        {
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"{GrammarNodeToStringConverter.Convert ( this.InnerNode )}?";
    }
}
