using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negative lookahead.
    /// </summary>
    public sealed class NegativeLookahead : GrammarNodeContainer<Char>
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
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        public static PositiveLookahead operator ! ( NegativeLookahead negativeLookahead )
        {
            if ( negativeLookahead is null )
                throw new ArgumentNullException ( nameof ( negativeLookahead ) );
            return new PositiveLookahead ( negativeLookahead.InnerNode );
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?!{GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }
}
