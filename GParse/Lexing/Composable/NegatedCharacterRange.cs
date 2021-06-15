using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that does not match an inclusive range
    /// </summary>
    public sealed class NegatedCharacterRange : GrammarNode<Char>, IEquatable<NegatedCharacterRange?>
    {
        /// <summary>
        /// The character range not matched by this node.
        /// </summary>
        public Range<Char> Range { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNegatedRange;

        /// <summary>
        /// Initializes a new negated character range grammar node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public NegatedCharacterRange(Char start, Char end) : this(new Range<Char>(start, end))
        {
        }

        /// <summary>
        /// Initializes a new negated character range grammar node.
        /// </summary>
        /// <param name="range"></param>
        public NegatedCharacterRange(Range<Char> range)
        {
            this.Range = range;
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) => this.Equals(obj as NegatedCharacterRange);

        /// <inheritdoc/>
        public Boolean Equals(NegatedCharacterRange? other) =>
            other != null
            && this.Range.Equals(other.Range);

        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            HashCode.Combine(this.Range);

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString() =>
            $"[^{CharUtils.ToReadableString(this.Range.Start)}-{CharUtils.ToReadableString(this.Range.End)}]";

        /// <summary>
        /// Negates this negated character range.
        /// </summary>
        /// <param name="negatedRange"></param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead.")]
        public static CharacterRange operator !(NegatedCharacterRange negatedRange)
        {
            if (negatedRange is null)
                throw new ArgumentNullException(nameof(negatedRange));
            return new CharacterRange(negatedRange.Range);
        }

        /// <summary>
        /// Checks whether two negated character ranges are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(NegatedCharacterRange? left, NegatedCharacterRange? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two negated character ranges are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(NegatedCharacterRange? left, NegatedCharacterRange? right) =>
            !(left == right);
    }
}