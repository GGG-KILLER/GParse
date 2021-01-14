using System;

namespace GParse
{
    /// <summary>
    /// Defines a point in a source code file
    /// </summary>
    public class SourceLocation : IEquatable<SourceLocation>, IComparable<SourceLocation>
    {
        /// <summary>
        /// The start of a file
        /// </summary>
        public static readonly SourceLocation StartOfFile = new ( 1, 1, 0 );

        /// <summary>
        /// Calculates the location of a given offset in a string.
        /// </summary>
        /// <param name="input">The string to calculate the location on.</param>
        /// <param name="position">The (absolute) position to calculate the location of.</param>
        /// <param name="reference">The location located before the position to use as a reference.</param>
        /// <returns>The calculated location.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the <paramref name="position"/> is less than 0 or greater than the
        /// <paramref name="input"/> length.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="reference"/> is located after the <paramref name="position"/>.
        /// </exception>
        public static SourceLocation Calculate ( String input, Int32 position, SourceLocation reference )
        {
            if ( input is null )
                throw new ArgumentNullException ( nameof ( input ) );
            if ( reference is null )
                throw new ArgumentNullException ( nameof ( reference ) );
            if ( position < 0 )
                throw new ArgumentOutOfRangeException ( nameof ( position ), "The position must be positive." );
            if ( position > input.Length )
                throw new ArgumentOutOfRangeException ( nameof ( position ), "The position is outside the string." );
            if ( position < reference.Byte )
                throw new ArgumentException ( "The reference location is located after the position.", nameof ( reference ) );
            if ( position == reference.Byte )
                return reference;

            var line = reference.Line;
            var column = reference.Column;
            for ( var i = reference.Byte; i < position; i++ )
            {
                if ( input[i] == '\n' )
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }
            return new SourceLocation ( line, column, position );
        }

        /// <summary>
        /// <inheritdoc cref="Calculate(String, Int32, SourceLocation)"/>
        /// </summary>
        /// <param name="input"><inheritdoc cref="Calculate(String, Int32, SourceLocation)"/></param>
        /// <param name="position"><inheritdoc cref="Calculate(String, Int32, SourceLocation)"/></param>
        /// <returns><inheritdoc cref="Calculate(String, Int32, SourceLocation)"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="Calculate(String, Int32, SourceLocation)"/></exception>
        public static SourceLocation Calculate ( String input, Int32 position ) =>
            Calculate ( input, position, StartOfFile );

        /// <summary>
        /// The byte offset of this location
        /// </summary>
        public Int32 Byte { get; }

        /// <summary>
        /// The line of this location
        /// </summary>
        public Int32 Line { get; }

        /// <summary>
        /// The column of this location
        /// </summary>
        public Int32 Column { get; }

        /// <summary>
        /// Initializes this location
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="pos"></param>
        public SourceLocation ( Int32 line, Int32 column, Int32 pos )
        {
            this.Line = line;
            this.Column = column;
            this.Byte = pos;
        }

        /// <summary>
        /// Creates a range with this as start and
        /// <paramref name="end" /> as end
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public SourceRange To ( SourceLocation end ) => new ( this, end );

        /// <inheritdoc />
        public override String ToString ( ) => $"{this.Line}:{this.Column}";

        /// <summary>
        /// Deconstructs this source position
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Column"></param>
        public void Deconstruct ( out Int32 Line, out Int32 Column )
        {
            Line = this.Line;
            Column = this.Column;
        }

        /// <summary>
        /// Deconstructs this source position
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Column"></param>
        /// <param name="Byte"></param>
        public void Deconstruct ( out Int32 Line, out Int32 Column, out Int32 Byte )
        {
            Line = this.Line;
            Column = this.Column;
            Byte = this.Byte;
        }

        /// <inheritdoc/>
        public Int32 CompareTo ( SourceLocation? other )
        {
            if ( ReferenceEquals ( this, other ) ) return 0;
            else if ( other is null ) return 1;
            else return this.Byte.CompareTo ( other.Byte );
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is SourceLocation location && this.Equals ( location );

        /// <inheritdoc />
        public Boolean Equals ( SourceLocation? other ) =>
            other is not null && this.Column == other.Column && this.Line == other.Line && this.Byte == other.Byte;

        /// <inheritdoc />
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.Line, this.Column, this.Byte );

        /// <summary>
        /// Checks whether two locations are equal.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator == ( SourceLocation? lhs, SourceLocation? rhs )
        {
            if ( rhs is null ) return lhs is null;
            return ReferenceEquals ( lhs, rhs ) || rhs.Equals ( lhs );
        }

        /// <summary>
        /// Checks whether two locations are not equal.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator != ( SourceLocation? lhs, SourceLocation? rhs ) => !( lhs == rhs );

        /// <summary>
        /// Checks whether a given location is less than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator < ( SourceLocation? left, SourceLocation? right )
        {
            if ( left is null ) return right is not null;
            return left.CompareTo ( right ) < 0;
        }

        /// <summary>
        /// Checks whether a given location is less than or equal to another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator <= ( SourceLocation? left, SourceLocation? right ) =>
            left is null || left.CompareTo ( right ) <= 0;

        /// <summary>
        /// Checks whether a given location is greater than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator > ( SourceLocation? left, SourceLocation? right ) => right < left;

        /// <summary>
        /// Checks whether a given location is greater than or equal than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator >= ( SourceLocation? left, SourceLocation? right ) => right <= left;

        #endregion Generated Code
    }
}
