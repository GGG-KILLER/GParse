using System;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Composable
{
    /// <summary>
    /// The base class for all grammar nodes
    /// </summary>
    public abstract class GrammarNode<T>
    {
        /// <summary>
        /// This node's kind.
        /// </summary>
        public abstract GrammarNodeKind Kind { get; }

        /// <inheritdoc cref="operator |(GrammarNode{T}, GrammarNode{T})" />
        /// <param name="other">The node to create an alternation with.</param>
        public virtual Alternation<T> OrThen(GrammarNode<T> other) =>
            new(this, other);

        /// <inheritdoc cref="operator &amp;(GrammarNode{T}, GrammarNode{T})" />
        /// <param name="other">The node to create a sequence with.</param>
        public virtual Sequence<T> AndThen(GrammarNode<T> other) =>
            new(this, other);

        /// <inheritdoc cref="operator *(GrammarNode{T}, UInt32)" />
        public virtual Repetition<T> Repeatedly(UInt32 repetitions) =>
            new(this, new RepetitionRange(repetitions, repetitions), false);

        /// <inheritdoc cref="operator *(GrammarNode{T}, RepetitionRange)" />
        public virtual Repetition<T> Repeatedly(RepetitionRange repetitions) =>
            new(this, repetitions, false);

        /// <summary>
        /// Creates an alternation node.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Has OrThen method.")]
        public static Alternation<T> operator |(GrammarNode<T> left, GrammarNode<T> right)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            if (right is null)
                throw new ArgumentNullException(nameof(right));
            return left.OrThen(right);
        }

        /// <summary>
        /// Creates a sequence node.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Has AndThen method.")]
        public static Sequence<T> operator &(GrammarNode<T> left, GrammarNode<T> right)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            if (right is null)
                throw new ArgumentNullException(nameof(right));
            return left.AndThen(right);
        }

        /// <summary>
        /// Creates a repetition node.
        /// </summary>
        /// <param name="left">The node to be matched.</param>
        /// <param name="repetitions">The number of times the node should be matched.</param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Has Repeatedly method.")]
        public static Repetition<T> operator *(GrammarNode<T> left, UInt32 repetitions)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            return left.Repeatedly(repetitions);
        }

        /// <summary>
        /// Creates a repetition node.
        /// </summary>
        /// <param name="left">The node to be matched.</param>
        /// <param name="repetitions">The range of times the node should be matched.</param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Has Repeatedly method.")]
        public static Repetition<T> operator *(GrammarNode<T> left, RepetitionRange repetitions)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            return left.Repeatedly(repetitions);
        }
    }
}