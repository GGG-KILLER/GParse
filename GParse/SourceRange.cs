using System;
using System.Collections.Generic;
using GParse.Math;

namespace GParse
{
    /// <summary>
    /// Defines a range in source code
    /// </summary>
    public readonly struct SourceRange : IEquatable<SourceRange>
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly SourceRange Zero = new SourceRange ( SourceLocation.Zero, SourceLocation.Zero );

        /// <summary>
        /// Translates a position <see cref="Range{T}"/> to a <see cref="SourceRange"/>.
        /// </summary>
        /// <param name="input">The string to calculate the locations on.</param>
        /// <param name="range">The (absolute) position range to calculate the locations of.</param>
        /// <param name="reference">A location located earlier than the range to use as a reference.</param>
        /// <returns>The calculated location range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when any of the <paramref name="range"/>'s positions are less than zero or greater
        /// or equal to the <paramref name="input"/> length.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="reference"/> location is located before any of the
        /// <paramref name="range"/>'s positions.
        /// </exception>
        public static SourceRange Calculate ( String input, Range<Int32> range, SourceLocation reference )
        {
            var startPosition = SourceLocation.Calculate ( input, range.Start, reference );
            var endPosition = SourceLocation.Calculate ( input, range.End, startPosition );
            return new SourceRange ( startPosition, endPosition );
        }

        /// <summary>
        /// <inheritdoc cref="Calculate(String, Range{Int32}, SourceLocation)"/>
        /// </summary>
        /// <param name="input"><inheritdoc cref="Calculate(String, Range{Int32}, SourceLocation)"/></param>
        /// <param name="range"><inheritdoc cref="Calculate(String, Range{Int32}, SourceLocation)"/></param>
        /// <returns><inheritdoc cref="Calculate(String, Range{Int32}, SourceLocation)"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="Calculate(String, Range{Int32}, SourceLocation)"/></exception>
        public static SourceRange Calculate ( String input, Range<Int32> range ) =>
            Calculate ( input, range, SourceLocation.Zero );

        /// <summary>
        /// Starting location
        /// </summary>
        public readonly SourceLocation Start;

        /// <summary>
        /// Ending location
        /// </summary>
        public readonly SourceLocation End;

        /// <summary>
        /// Initializes this range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public SourceRange ( SourceLocation start, SourceLocation end )
        {
            this.End = end;
            this.Start = start;
        }

        /// <inheritdoc />
        public override String ToString ( ) => $"{this.Start} - {this.End}";

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is SourceRange range && this.Equals ( range );

        /// <inheritdoc />
        public Boolean Equals ( SourceRange other ) => this.End.Equals ( other.End )
                    && this.Start.Equals ( other.Start );

        /// <inheritdoc />
        public override Int32 GetHashCode ( )
        {
            var hashCode = 945720665;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.End );
            return ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.Start );
        }

        /// <inheritdoc />
        public static Boolean operator == ( SourceRange lhs, SourceRange rhs ) => lhs.Equals ( rhs );

        /// <inheritdoc />
        public static Boolean operator != ( SourceRange lhs, SourceRange rhs ) => !( lhs == rhs );

        #endregion Generated Code
    }
}
