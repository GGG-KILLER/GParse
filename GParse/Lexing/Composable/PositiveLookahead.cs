using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a positive lookahead.
    /// </summary>
    public sealed class PositiveLookahead : GrammarNodeContainer<Char>, IEquatable<PositiveLookahead?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterPositiveLookahead;

        /// <summary>
        /// Initializes a new lookahead.
        /// </summary>
        /// <param name="node"></param>
        public PositiveLookahead(GrammarNode<Char> node) : base(node)
        {
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            this.Equals(obj as PositiveLookahead);

        /// <inheritdoc/>
        public Boolean Equals(PositiveLookahead? other) =>
            other != null
            && EqualityComparer<GrammarNode<Char>>.Default.Equals(this.InnerNode, other.InnerNode);

        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            HashCode.Combine(this.InnerNode);

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString() =>
            $"(?={GrammarNodeToStringConverter.Convert(this.InnerNode)})";

        /// <summary>
        /// Negates a positive lookahead.
        /// </summary>
        /// <param name="positiveLookahead"></param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead.")]
        public static NegativeLookahead operator !(PositiveLookahead positiveLookahead)
        {
            if (positiveLookahead is null)
                throw new ArgumentNullException(nameof(positiveLookahead));
            return new NegativeLookahead(positiveLookahead.InnerNode);
        }

        /// <summary>
        /// Checks whether two positive lookaheads are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(PositiveLookahead? left, PositiveLookahead? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two positive lookaheads are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(PositiveLookahead? left, PositiveLookahead? right) =>
            !(left == right);
    }
}