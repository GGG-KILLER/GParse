using System;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Composable
{
    /// <summary>
    /// The range of a repetition.
    /// </summary>
    // We can't use Range<T> because nullable types cannot satisfy any interface requirements
    public readonly struct RepetitionRange : IEquatable<RepetitionRange>
    {
        /// <summary>
        /// The minimum number of matches.
        /// </summary>
        public UInt32 Minimum { get; }

        /// <summary>
        /// THe maximum number of matches (null represents an open end).
        /// </summary>
        public UInt32? Maximum { get; }

        /// <summary>
        /// Whether this is a finite range (has both ends defined).
        /// </summary>
        public Boolean IsFinite => this.Maximum.HasValue;

        /// <summary>
        /// Whether this range contains a single element.
        /// </summary>
        public Boolean IsSingleElement => this.IsFinite && this.Minimum == this.Maximum;

        /// <summary>
        /// Initializes a new repetition range.
        /// </summary>
        /// <param name="minimum">The range's lower end.</param>
        /// <param name="maximum">The range's upper end.</param>
        public RepetitionRange(UInt32 minimum, UInt32? maximum)
        {
            if (maximum.HasValue && maximum < minimum)
                throw new ArgumentOutOfRangeException(nameof(maximum), "The maximum number of repetitions must be larger than the minimum.");
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Deconstructs this struct into a variable pair
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public void Deconstruct(out UInt32 minimum, out UInt32? maximum)
        {
            minimum = this.Minimum;
            maximum = this.Maximum;
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals(Object? obj) => obj is RepetitionRange range && this.Equals(range);

        /// <inheritdoc />
        public Boolean Equals(RepetitionRange other) => this.Minimum == other.Minimum && this.Maximum == other.Maximum;

        /// <inheritdoc />
        public override Int32 GetHashCode() =>
            HashCode.Combine(this.Minimum, this.Maximum);

        /// <summary>
        /// Checks whether this range is equal to another.
        /// </summary>
        /// <param name="left">The range on the left side of the operator.</param>
        /// <param name="right">The range on the right side of the operator.</param>
        /// <returns>Whether this range is equal to another.</returns>
        public static Boolean operator ==(RepetitionRange left, RepetitionRange right) => left.Equals(right);

        /// <summary>
        /// Checks whether this range is not equal to other.
        /// </summary>
        /// <param name="left">The range on the left side of the operator.</param>
        /// <param name="right">The range on the right side of the operator.</param>
        /// <returns>Whether this range is not equal to other.</returns>
        public static Boolean operator !=(RepetitionRange left, RepetitionRange right) => !(left == right);

        #endregion Generated Code

#if HAS_VALUETUPLE
        /// <summary>
        /// Converts a tuple-based range into this type.
        /// </summary>
        /// <param name="value">The tuple-based range.</param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "They can use the constructor instead." )]
        public static implicit operator RepetitionRange ( (UInt32 Minimum, UInt32? Maximum) value ) =>
            new ( value.Minimum, value.Maximum );
#endif
    }
}