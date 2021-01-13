using System;
using System.Collections.Generic;
using GParse.Math;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a repetition of a grammar node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Repetition<T> : GrammarNodeContainer<T>, IEquatable<Repetition<T>?>
    {
        private static GrammarNode<T> GetInnerNode ( GrammarNode<T> grammarNode, ref RepetitionRange range, Boolean isLazy )
        {
            if ( grammarNode is Repetition<T> repetition && repetition.IsLazy == isLazy )
            {
                /* expr{α}{β} ≡ expr{α·β} */
                if ( repetition.Range.IsSingleElement && range.IsSingleElement )
                {
                    var reps = repetition.Range.Minimum * range.Minimum;
                    range = new RepetitionRange ( reps, reps );
                    return repetition.InnerNode;
                }

                /* expr{a, b}{c, d} ≡ expr{a·c, b·d} IF b·c ≥ a·(c + 1) - 1
                 * Basically checks if each occurrence of the range [n·a, n·b] where n ∈ [c, d]
                 * overlaps with each other.
                 * If it does, we then just get the range of [a·c, b·d] (the union of all
                 * occurrences of the range on the previous sentence) instead of nesting the repetitions.
                 */
                if ( repetition.Range.IsFinite
                      && range.IsFinite
                      && repetition.Range.Maximum * range.Minimum >= repetition.Range.Minimum * ( range.Minimum + 1 ) - 1 )
                {
                    range = new RepetitionRange (
                        SaturatingMath.Multiply ( repetition.Range.Minimum, range.Minimum ),
                        SaturatingMath.Multiply ( repetition.Range.Maximum!.Value, range.Maximum!.Value ) );
                    return repetition.InnerNode;
                }
            }

            return grammarNode;
        }

        /// <summary>
        /// The number of repetitions required and permitted.
        /// </summary>
        public RepetitionRange Range { get; }

        /// <summary>
        /// Whether this node works lazily by matching as few matches as possible.
        /// </summary>
        public Boolean IsLazy { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.Repetition;

        /// <summary>
        /// Creates a new repetition node
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="range"></param>
        /// <param name="isLazy"></param>
        public Repetition ( GrammarNode<T> grammarNode, RepetitionRange range, Boolean isLazy )
            : base ( GetInnerNode ( grammarNode, ref range, isLazy ) )
        {
            this.Range = range;
            this.IsLazy = isLazy;
        }

        /// <summary>
        /// Transforms this repetition into a lazy one.
        /// Returns the same instance if it's already lazy.
        /// </summary>
        /// <returns></returns>
        public Repetition<T> Lazily ( ) =>
            this.IsLazy ? this : new Repetition<T> ( this.InnerNode, this.Range, true );

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as Repetition<T> );

        /// <inheritdoc/>
        public Boolean Equals ( Repetition<T>? other ) =>
            other != null
            && EqualityComparer<GrammarNode<T>>.Default.Equals ( this.InnerNode, other.InnerNode )
            && this.Range.Equals ( other.Range )
            && this.IsLazy == other.IsLazy;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.Kind, this.InnerNode, this.Range, this.IsLazy, this.Kind );

        /// <summary>
        /// Checks whether two repetitions are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( Repetition<T>? left, Repetition<T>? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two repetitions are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Repetition<T>? left, Repetition<T>? right ) =>
            !( left == right );
    }
}