using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negative lookahead.
    /// </summary>
    public sealed class NegativeLookahead : GrammarNodeContainer<Char>, IEquatable<NegativeLookahead?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNegativeLookahead;

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

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as NegativeLookahead );

        /// <inheritdoc/>
        public Boolean Equals ( NegativeLookahead? other ) =>
            other != null
            && EqualityComparer<GrammarNode<Char>>.Default.Equals ( this.InnerNode, other.InnerNode );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.InnerNode );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?!{GrammarNodeToStringConverter.Convert ( this.InnerNode )})";

        /// <summary>
        /// Checks whether two negative lookaheads are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NegativeLookahead? left, NegativeLookahead? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two negative lookaheads are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NegativeLookahead? left, NegativeLookahead? right ) => !( left == right );
    }
}
