namespace GParse.Math
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An inclusive <see cref="UInt32" /> inclusive range
    /// </summary>
    public readonly partial struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        /// <summary>
        /// Starting location of the range
        /// </summary>
        public readonly T Start;

        /// <summary>
        /// Ending location of the range (inclusive)
        /// </summary>
        public readonly T End;

        /// <summary>
        /// Whether this range spans a single element
        /// </summary>
        public readonly Boolean IsSingle;

        /// <summary>
        /// Initializes a range that spans a single number
        /// </summary>
        /// <param name="single"></param>
        public Range ( T single )
        {
            this.Start = single;
            this.End = single;
            this.IsSingle = true;
        }

        /// <summary>
        /// Initializes a range with a start and end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Range ( T start, T end )
        {
            if ( start.CompareTo ( end ) == 1 )
                throw new InvalidOperationException ( "Cannot initialize a range with a start greater than the end" );

            this.Start = start;
            this.End = end;
            this.IsSingle = start.CompareTo ( end ) == 0;
        }

        /// <summary>
        /// Returns whether this <see cref="Range{T}" /> intersects with another <see
        /// cref="Range{T}" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean IntersectsWith ( Range<T> other ) =>
            this.ValueIn ( other.Start ) || this.ValueIn ( other.End );

        /// <summary>
        /// Joins this <see cref="Range{T}" /> with another <see cref="Range{T}" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Range<T> JoinWith ( Range<T> other )
        {
            T start = this.Start.CompareTo ( other.Start ) == -1 ? this.Start : other.Start;
            T end = this.End.CompareTo ( other.End ) == 1 ? other.End : this.End;
            return this.IntersectsWith ( other )
                ? new Range<T> ( start, end )
                : throw new InvalidOperationException ( "Cannot join two ranges that do not intersect" );
        }

        /// <summary>
        /// Returns whether a certain <paramref name="value" /> is contained inside this <see
        /// cref="Range{T}" />
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public Boolean ValueIn ( T value )
        {
            if ( typeof ( T ) == typeof ( Byte ) )
            {
                T start = this.Start;
                T end = this.End;
                var byteValue = Unsafe.As<T, Byte> ( ref value );
                var byteStart = Unsafe.As<T, Byte> ( ref start );
                var byteEnd = Unsafe.As<T, Byte> ( ref end );
                return ( UInt16 ) ( byteValue - byteStart ) <= ( byteEnd - byteStart );
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                T start = this.Start;
                T end = this.End;
                var u16Value = Unsafe.As<T, UInt16> ( ref value );
                var u16Start = Unsafe.As<T, UInt16> ( ref start );
                var u16End = Unsafe.As<T, UInt16> ( ref end );
                return ( UInt32 ) ( u16Value - u16Start ) <= ( u16End - u16Start );
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                T start = this.Start;
                T end = this.End;
                var charValue = Unsafe.As<T, Char> ( ref value );
                var charStart = Unsafe.As<T, Char> ( ref start );
                var charEnd = Unsafe.As<T, Char> ( ref end );
                return ( UInt32 ) ( charValue - charStart ) <= ( charEnd - charStart );
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                T start = this.Start;
                T end = this.End;
                var u32Value = Unsafe.As<T, UInt32> ( ref value );
                var u32Start = Unsafe.As<T, UInt32> ( ref start );
                var u32End = Unsafe.As<T, UInt32> ( ref end );
                return ( UInt64 ) ( u32Value - u32Start ) <= ( u32End - u32Start );
            }
            else
            {
                return this.Start.CompareTo ( value ) < 1 && value.CompareTo ( this.End ) < 1;
            }
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is Range<T> range && this.Equals ( range );

        /// <inheritdoc />
        public Boolean Equals ( Range<T> other ) =>
            EqualityComparer<T>.Default.Equals ( this.Start, other.Start ) && EqualityComparer<T>.Default.Equals ( this.End, other.End );


        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage ( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppression is valid for some target frameworks." )]
        [System.Diagnostics.CodeAnalysis.SuppressMessage ( "Style", "IDE0070:Use 'System.HashCode'", Justification = "We have to maintain consistent behavior between all target frameworks." )]
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode ( this.Start );
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode ( this.End );
            return hashCode;
        }

        /// <inheritdoc />
        public static Boolean operator == ( Range<T> left, Range<T> right ) => left.Equals ( right );

        /// <inheritdoc />
        public static Boolean operator != ( Range<T> left, Range<T> right ) => !( left == right );

        #endregion Generated Code
    }
}