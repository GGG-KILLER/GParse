using System;
using GParse.Math;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a repetition of a grammar node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Repetition<T> : GrammarNodeContainer<T>
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
                        SaturatingMath.Multiply ( repetition.Range.Minimum!.Value, range.Minimum!.Value ),
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
    }
}