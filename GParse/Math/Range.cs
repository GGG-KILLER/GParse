namespace GParse.Math
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An inclusive <see cref="UInt32" /> inclusive range.
    /// </summary>
    public readonly partial struct Range<T> : IEquatable<Range<T>>, IComparable<Range<T>> where T : IComparable<T>
    {
        /// <summary>
        /// Starting location of the range.
        /// </summary>
        public readonly T Start;

        /// <summary>
        /// Ending location of the range (inclusive).
        /// </summary>
        public readonly T End;

        /// <summary>
        /// Whether this range spans a single element.
        /// </summary>
        public readonly Boolean IsSingle;

        /// <summary>
        /// Initializes a range that spans a single number.
        /// </summary>
        /// <param name="single"></param>
        public Range ( T single )
        {
            this.Start = single;
            this.End = single;
            this.IsSingle = true;
        }

        /// <summary>
        /// Initializes a range with a start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Range ( T start, T end )
        {
            if ( start.CompareTo ( end ) == 1 )
                throw new InvalidOperationException ( "Cannot initialize a range with a start greater than the end" );

            this.Start = start;
            this.End = end;
            this.IsSingle = EqualityComparer<T>.Default.Equals ( start, end ) || start.CompareTo ( end ) == 0;
        }

        /// <summary>
        /// Returns whether this <see cref="Range{T}" /> intersects with another <see
        /// cref="Range{T}" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean IntersectsWith ( Range<T> other )
        {
            T start = this.Start;
            T end = this.End;
            T otherStart = other.Start;
            T otherEnd = other.End;
            if ( typeof ( T ) == typeof ( SByte ) )
            {
                var i8Start = Unsafe.As<T, SByte> ( ref start );
                var i8End = Unsafe.As<T, SByte> ( ref end );
                var i8OtherStart = Unsafe.As<T, SByte> ( ref otherStart );
                var i8OtherEnd = Unsafe.As<T, SByte> ( ref otherEnd );
                return i8Start <= i8OtherEnd && i8OtherStart <= i8End;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8Start = Unsafe.As<T, Byte> ( ref start );
                var u8End = Unsafe.As<T, Byte> ( ref end );
                var u8OtherStart = Unsafe.As<T, Byte> ( ref otherStart );
                var u8OtherEnd = Unsafe.As<T, Byte> ( ref otherEnd );
                return u8Start <= u8OtherEnd && u8OtherStart <= u8End;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charStart = Unsafe.As<T, Char> ( ref start );
                var charEnd = Unsafe.As<T, Char> ( ref end );
                var charOtherStart = Unsafe.As<T, Char> ( ref otherStart );
                var charOtherEnd = Unsafe.As<T, Char> ( ref otherEnd );
                return charStart <= charOtherEnd && charOtherStart <= charEnd;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16Start = Unsafe.As<T, Int16> ( ref start );
                var i16End = Unsafe.As<T, Int16> ( ref end );
                var i16OtherStart = Unsafe.As<T, Int16> ( ref otherStart );
                var i16OtherEnd = Unsafe.As<T, Int16> ( ref otherEnd );
                return i16Start <= i16OtherEnd && i16OtherStart <= i16End;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16Start = Unsafe.As<T, UInt16> ( ref start );
                var u16End = Unsafe.As<T, UInt16> ( ref end );
                var u16OtherStart = Unsafe.As<T, UInt16> ( ref otherStart );
                var u16OtherEnd = Unsafe.As<T, UInt16> ( ref otherEnd );
                return u16Start <= u16OtherEnd && u16OtherStart <= u16End;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32Start = Unsafe.As<T, Int32> ( ref start );
                var i32End = Unsafe.As<T, Int32> ( ref end );
                var i32OtherStart = Unsafe.As<T, Int32> ( ref otherStart );
                var i32OtherEnd = Unsafe.As<T, Int32> ( ref otherEnd );
                return i32Start <= i32OtherEnd && i32OtherStart <= i32End;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32Start = Unsafe.As<T, UInt32> ( ref start );
                var u32End = Unsafe.As<T, UInt32> ( ref end );
                var u32OtherStart = Unsafe.As<T, UInt32> ( ref otherStart );
                var u32OtherEnd = Unsafe.As<T, UInt32> ( ref otherEnd );
                return u32Start <= u32OtherEnd && u32OtherStart <= u32End;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64Start = Unsafe.As<T, Int64> ( ref start );
                var i64End = Unsafe.As<T, Int64> ( ref end );
                var i64OtherStart = Unsafe.As<T, Int64> ( ref otherStart );
                var i64OtherEnd = Unsafe.As<T, Int64> ( ref otherEnd );
                return i64Start <= i64OtherEnd && i64OtherStart <= i64End;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64Start = Unsafe.As<T, UInt64> ( ref start );
                var u64End = Unsafe.As<T, UInt64> ( ref end );
                var u64OtherStart = Unsafe.As<T, UInt64> ( ref otherStart );
                var u64OtherEnd = Unsafe.As<T, UInt64> ( ref otherEnd );
                return u64Start <= u64OtherEnd && u64OtherStart <= u64End;
            }
            else
            {
                return start.CompareTo ( otherEnd ) < 1 && otherStart.CompareTo ( end ) < 1;
            }
        }

        /// <summary>
        /// Joins this <see cref="Range{T}" /> with another <see cref="Range{T}" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Range<T> JoinWith ( Range<T> other )
        {
            return this.IntersectsWith ( other )
                ? new Range<T> ( Min ( this.Start, other.Start ), Max ( this.End, other.End ) )
                : throw new InvalidOperationException ( "Cannot join two ranges that do not intersect" );
        }

        /// <summary>
        /// Returns whether a certain <paramref name="value" /> is contained inside this <see
        /// cref="Range{T}" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public Boolean ValueIn ( T value )
        {
            T start = this.Start;
            T end = this.End;
            if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8Value = Unsafe.As<T, Byte> ( ref value );
                var u8Start = Unsafe.As<T, Byte> ( ref start );
                var u8End = Unsafe.As<T, Byte> ( ref end );
                return ( UInt16 ) ( u8Value - u8Start ) <= ( u8End - u8Start );
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16Value = Unsafe.As<T, UInt16> ( ref value );
                var u16Start = Unsafe.As<T, UInt16> ( ref start );
                var u16End = Unsafe.As<T, UInt16> ( ref end );
                return ( UInt32 ) ( u16Value - u16Start ) <= ( u16End - u16Start );
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charValue = Unsafe.As<T, Char> ( ref value );
                var charStart = Unsafe.As<T, Char> ( ref start );
                var charEnd = Unsafe.As<T, Char> ( ref end );
                return ( UInt32 ) ( charValue - charStart ) <= ( charEnd - charStart );
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32Value = Unsafe.As<T, UInt32> ( ref value );
                var u32Start = Unsafe.As<T, UInt32> ( ref start );
                var u32End = Unsafe.As<T, UInt32> ( ref end );
                return ( UInt64 ) ( u32Value - u32Start ) <= ( u32End - u32Start );
            }
            else
            {
                return start.CompareTo ( value ) < 1 && value.CompareTo ( end ) < 1;
            }
        }

        /// <inheritdoc/>
        public Int32 CompareTo ( Range<T> other )
        {
            var cmp = this.Start.CompareTo ( other.Start );
            if ( cmp == 0 )
                cmp = this.End.CompareTo ( other.End );
            return cmp;
        }

        /// <summary>
        /// Deconstructs this range into its respective parts.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Deconstruct ( out T start, out T end )
        {
            start = this.Start;
            end = this.End;
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is Range<T> range && this.Equals ( range );

        /// <inheritdoc />
        public Boolean Equals ( Range<T> other ) =>
            EqualityComparer<T>.Default.Equals ( this.Start, other.Start )
            && EqualityComparer<T>.Default.Equals ( this.End, other.End );


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

#if HAS_VALUETUPLE
        /// <summary>
        /// Converts a tuple into a range.
        /// </summary>
        /// <param name="range"></param>
        public static implicit operator Range<T> ( (T start, T end) range ) =>
            new Range<T> ( range.start, range.end );
#endif

        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static T Min ( T x, T y )
        {
            if ( typeof ( T ) == typeof ( SByte ) )
            {
                var i8x = Unsafe.As<T, SByte> ( ref x );
                var i8y = Unsafe.As<T, SByte> ( ref y );
                return i8x <= i8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8x = Unsafe.As<T, Byte> ( ref x );
                var u8y = Unsafe.As<T, Byte> ( ref y );
                return u8x <= u8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charx = Unsafe.As<T, Char> ( ref x );
                var chary = Unsafe.As<T, Char> ( ref y );
                return charx <= chary ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16x = Unsafe.As<T, Int16> ( ref x );
                var i16y = Unsafe.As<T, Int16> ( ref y );
                return i16x <= i16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16x = Unsafe.As<T, UInt16> ( ref x );
                var u16y = Unsafe.As<T, UInt16> ( ref y );
                return u16x <= u16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32x = Unsafe.As<T, Int32> ( ref x );
                var i32y = Unsafe.As<T, Int32> ( ref y );
                return i32x <= i32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32x = Unsafe.As<T, UInt32> ( ref x );
                var u32y = Unsafe.As<T, UInt32> ( ref y );
                return u32x <= u32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64x = Unsafe.As<T, Int64> ( ref x );
                var i64y = Unsafe.As<T, Int64> ( ref y );
                return i64x <= i64y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64x = Unsafe.As<T, UInt64> ( ref x );
                var u64y = Unsafe.As<T, UInt64> ( ref y );
                return u64x <= u64y ? x : y;
            }
            else
            {
                return x.CompareTo ( y ) <= 0 ? x : y;
            }
        }

        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static T Max ( T x, T y )
        {
            if ( typeof ( T ) == typeof ( SByte ) )
            {
                var i8x = Unsafe.As<T, SByte> ( ref x );
                var i8y = Unsafe.As<T, SByte> ( ref y );
                return i8x >= i8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8x = Unsafe.As<T, Byte> ( ref x );
                var u8y = Unsafe.As<T, Byte> ( ref y );
                return u8x >= u8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charx = Unsafe.As<T, Char> ( ref x );
                var chary = Unsafe.As<T, Char> ( ref y );
                return charx >= chary ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16x = Unsafe.As<T, Int16> ( ref x );
                var i16y = Unsafe.As<T, Int16> ( ref y );
                return i16x >= i16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16x = Unsafe.As<T, UInt16> ( ref x );
                var u16y = Unsafe.As<T, UInt16> ( ref y );
                return u16x >= u16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32x = Unsafe.As<T, Int32> ( ref x );
                var i32y = Unsafe.As<T, Int32> ( ref y );
                return i32x >= i32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32x = Unsafe.As<T, UInt32> ( ref x );
                var u32y = Unsafe.As<T, UInt32> ( ref y );
                return u32x >= u32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64x = Unsafe.As<T, Int64> ( ref x );
                var i64y = Unsafe.As<T, Int64> ( ref y );
                return i64x >= i64y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64x = Unsafe.As<T, UInt64> ( ref x );
                var u64y = Unsafe.As<T, UInt64> ( ref y );
                return u64x >= u64y ? x : y;
            }
            else
            {
                return x.CompareTo ( y ) >= 0 ? x : y;
            }
        }
    }
}