using System;
using System.Collections.Generic;
using System.Text;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negative lookahead.
    /// </summary>
    public class NegativeLookahead : GrammarNodeContainer<Char>
    {
        /// <summary>
        /// Initializes a new negative lookahead.
        /// </summary>
        /// <param name="innerNode"></param>
        public NegativeLookahead ( GrammarNode<Char> innerNode ) : base ( innerNode )
        {
        }

        /// <summary>
        /// Negates this lookahead.
        /// </summary>
        /// <param name="negativeLookahead"></param>
        /// <returns></returns>
        public static Lookahead operator ! ( NegativeLookahead negativeLookahead ) =>
            new Lookahead ( negativeLookahead.InnerNode );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?!{GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }
}
