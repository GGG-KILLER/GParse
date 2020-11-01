using System;

namespace GParse
{
    /// <summary>
    /// Defines a point in a source code file
    /// </summary>
    public readonly struct SourceLocation : IEquatable<SourceLocation>, IComparable<SourceLocation>
    {
        /// <summary>
        /// The start of a file
        /// </summary>
        public static readonly SourceLocation Zero = new SourceLocation ( 1, 1, 0 );

        /// <summary>
        /// Maximum possible value
        /// </summary>
        public static readonly SourceLocation Max = new SourceLocation ( Int32.MaxValue, Int32.MaxValue, Int32.MaxValue );

        /// <summary>
        /// Minimum possible value (invalid)
        /// </summary>
        public static readonly SourceLocation Min = new SourceLocation ( Int32.MinValue, Int32.MinValue, Int32.MinValue );

        /// <summary>
        /// Standard invalid location
        /// </summary>
        public static readonly SourceLocation Invalid = new SourceLocation ( -1, -1, -1 );

        /// <summary>
        /// Calculates the location of a given offset in a string.
        /// </summary>
        /// <param name="input">The string to calculate the location on.</param>
        /// <param name="offset">The offset to calculate the position of.</param>
        /// <param name="location">THe location to start calculating at.</param>
        /// <returns></returns>
        public static SourceLocation Calculate ( String input, Int32 offset, SourceLocation location )
        {
            if ( offset < 0 )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "The offset must be positive." );
            if ( offset > input.Length - location.Byte )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "Offset is too big." );

            var line = location.Line;
            var column = location.Column;
            var lastIdx = location.Byte + offset - 1;
            for ( var i = location.Byte; i <= lastIdx; i++ )
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
            return new SourceLocation ( line, column, location.Byte + offset );
        }

        /// <inheritdoc cref="Calculate(String, Int32, SourceLocation)"/>
        public static SourceLocation Calculate ( String input, Int32 offset ) =>
            Calculate ( input, offset, Zero );

        /// <summary>
        /// The byte offset of this location
        /// </summary>
        public readonly Int32 Byte;

        /// <summary>
        /// The line of this location
        /// </summary>
        public readonly Int32 Line;

        /// <summary>
        /// The column of this location
        /// </summary>
        public readonly Int32 Column;

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
        public SourceRange To ( SourceLocation end ) => new SourceRange ( this, end );

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
        public Int32 CompareTo ( SourceLocation other ) => this.Byte.CompareTo ( other.Byte );

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is SourceLocation location && this.Equals ( location );

        /// <inheritdoc />
        public Boolean Equals ( SourceLocation other ) =>
            this.Column == other.Column && this.Line == other.Line && this.Byte == other.Byte;

        /// <inheritdoc />
        public override Int32 GetHashCode ( )
        {
            var hashCode = 412437926;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + this.Column.GetHashCode ( );
            return ( hashCode * -1521134295 ) + this.Line.GetHashCode ( );
        }

        /// <summary>
        /// Checks whether two locations are equal.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator == ( SourceLocation lhs, SourceLocation rhs ) => lhs.Equals ( rhs );

        /// <summary>
        /// Checks whether two locations are not equal.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator != ( SourceLocation lhs, SourceLocation rhs ) => !( lhs == rhs );

        /// <summary>
        /// Checks whether a given location is less than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator < ( SourceLocation left, SourceLocation right ) => left.CompareTo ( right ) < 0;

        /// <summary>
        /// Checks whether a given location is less than or equal to another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator <= ( SourceLocation left, SourceLocation right ) => left.CompareTo ( right ) <= 0;

        /// <summary>
        /// Checks whether a given location is greater than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator > ( SourceLocation left, SourceLocation right ) => left.CompareTo ( right ) > 0;

        /// <summary>
        /// Checks whether a given location is greater than or equal than another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator >= ( SourceLocation left, SourceLocation right ) => left.CompareTo ( right ) >= 0;

        #endregion Generated Code
    }
}
