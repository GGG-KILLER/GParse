using System;
using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// The base class for all grammar nodes
    /// </summary>
    public abstract class GrammarNode<T>
    {
        /// <inheritdoc cref="operator |(GrammarNode{T}, GrammarNode{T})" />
        /// <param name="other">The node to create an alternation with.</param>
        public Alternation<T> Or ( GrammarNode<T> other ) =>
            this | other;

        /// <inheritdoc cref="operator &amp;(GrammarNode{T}, GrammarNode{T})" />
        /// <param name="other">The node to create a sequence with.</param>
        public Sequence<T> And ( GrammarNode<T> other ) =>
            this & other;

        /// <inheritdoc cref="operator *(GrammarNode{T}, UInt32)" />
        public Repetition<T> Repeatedly ( UInt32 repetitions ) =>
            this * repetitions;

        /// <inheritdoc cref="operator *(GrammarNode{T}, RepetitionRange)" />
        public Repetition<T> Repeatedly ( (UInt32? minimum, UInt32? maximum) repetitions ) =>
            this * repetitions;

        /// <summary>
        /// Creates an alternation node.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Alternation<T> operator | ( GrammarNode<T> left, GrammarNode<T> right )
        {
            var nodes = new List<GrammarNode<T>> ( );
            if ( left is Alternation<T> leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Alternation<T> rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Alternation<T> ( nodes.ToArray ( ) );
        }

        /// <summary>
        /// Creates a sequence node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Sequence<T> operator & ( GrammarNode<T> left, GrammarNode<T> right )
        {
            var nodes = new List<GrammarNode<T>> ( );
            if ( left is Sequence<T> leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Sequence<T> rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Sequence<T> ( nodes.ToArray ( ) );
        }

        /// <summary>
        /// Creates a repetition node
        /// </summary>
        /// <param name="left">The node to be matched.</param>
        /// <param name="repetitions">The number of times the node should be matched.</param>
        /// <returns></returns>
        public static Repetition<T> operator * ( GrammarNode<T> left, UInt32 repetitions ) =>
            new Repetition<T> ( left, (repetitions, repetitions) );

        /// <summary>
        /// Creates a repetition node
        /// </summary>
        /// <param name="left">The node to be matched.</param>
        /// <param name="repetitions">The range of times the node should be matched.</param>
        /// <returns></returns>
        public static Repetition<T> operator * ( GrammarNode<T> left, RepetitionRange repetitions ) =>
            new Repetition<T> ( left, repetitions );
    }
}