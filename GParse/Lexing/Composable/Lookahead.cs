using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a lookahead.
    /// </summary>
    public class Lookahead : GrammarNodeContainer<Char>
    {
        /// <summary>
        /// Initializes a new lookahead.
        /// </summary>
        /// <param name="node"></param>
        public Lookahead ( GrammarNode<Char> node ) : base ( node )
        {
        }

        /// <summary>
        /// Negates a lookahead.
        /// </summary>
        /// <param name="lookahead"></param>
        /// <returns></returns>
        public static NegativeLookahead operator ! ( Lookahead lookahead ) =>
            new NegativeLookahead ( lookahead.InnerNode );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?={GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }
}
