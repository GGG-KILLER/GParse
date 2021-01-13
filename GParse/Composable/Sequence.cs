using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a sequence of grammar rules
    /// </summary>
    public sealed class Sequence<T> : GrammarNodeListContainer<Sequence<T>, T>, IEquatable<Sequence<T>?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.Sequence;

        /// <summary>
        /// Initializes a sequence.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Sequence ( params GrammarNode<T>[] grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <summary>
        /// Initializes a sequence.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Sequence ( IEnumerable<GrammarNode<T>> grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as Sequence<T> );

        /// <inheritdoc/>
        public Boolean Equals ( Sequence<T>? other ) =>
            other != null
            && this.GrammarNodes.SequenceEqual ( other.GrammarNodes );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.GrammarNodes );

        /// <summary>
        /// Checks whether two sequences are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( Sequence<T>? left, Sequence<T>? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two sequences are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Sequence<T>? left, Sequence<T>? right ) =>
            !( left == right );
    }
}
