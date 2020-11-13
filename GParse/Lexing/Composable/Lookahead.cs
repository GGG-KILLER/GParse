using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a lookahead.
    /// </summary>
    public sealed class Lookahead : GrammarNodeContainer<Char>
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
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        [return: NotNullIfNotNull ( "lookahead" )]
        public static NegativeLookahead? operator ! ( Lookahead? lookahead ) =>
            lookahead is null ? null : new NegativeLookahead ( lookahead.InnerNode );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?={GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }
}
