using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a positive lookahead.
    /// </summary>
    public sealed class PositiveLookahead : GrammarNodeContainer<Char>
    {
        /// <summary>
        /// Initializes a new lookahead.
        /// </summary>
        /// <param name="node"></param>
        public PositiveLookahead ( GrammarNode<Char> node ) : base ( node )
        {
        }

        /// <summary>
        /// Negates a positive lookahead.
        /// </summary>
        /// <param name="positiveLookahead"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        public static NegativeLookahead operator ! ( PositiveLookahead positiveLookahead )
        {
            if ( positiveLookahead is null )
                throw new ArgumentNullException ( nameof ( positiveLookahead ) );
            return new NegativeLookahead ( positiveLookahead.InnerNode );
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?={GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }
}
