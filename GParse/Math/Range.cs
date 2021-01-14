namespace GParse.Math
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An inclusive <see cref="UInt32" /> inclusive range.
    /// </summary>
    public readonly struct Range<T> : IEquatable<Range<T>>, IComparable<Range<T>> where T : IComparable<T>
    {
        /// <summary>
        /// Starting location of the range.
        /// </summary>
        public T Start { get; }

        /// <summary>
        /// Ending location of the range (inclusive).
        /// </summary>
        public T End { get; }

        /// <summary>
        /// Whether this range spans a single element.
        /// </summary>
        public Boolean IsSingle { get; }

        /// <summary>
        /// Initializes a range that spans a single number.
        /// </summary>
        /// <param name="value"></param>
        public Range ( T value )
        {
            this.Start = value;
            this.End = value;
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
            if ( typeof ( T ) == typeof ( SByte ) )
            {
                var i8Start = ( SByte ) ( Object ) this.Start;
                var i8End = ( SByte ) ( Object ) this.End;
                var i8OtherStart = ( SByte ) ( Object ) other.Start;
                var i8OtherEnd = ( SByte ) ( Object ) other.End;
                return i8Start <= i8OtherEnd && i8OtherStart <= i8End;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8Start = ( Byte ) ( Object ) this.Start;
                var u8End = ( Byte ) ( Object ) this.End;
                var u8OtherStart = ( Byte ) ( Object ) other.Start;
                var u8OtherEnd = ( Byte ) ( Object ) other.End;
                return u8Start <= u8OtherEnd && u8OtherStart <= u8End;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charStart = ( Char ) ( Object ) this.Start;
                var charEnd = ( Char ) ( Object ) this.End;
                var charOtherStart = ( Char ) ( Object ) other.Start;
                var charOtherEnd = ( Char ) ( Object ) other.End;
                return charStart <= charOtherEnd && charOtherStart <= charEnd;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16Start = ( Int16 ) ( Object ) this.Start;
                var i16End = ( Int16 ) ( Object ) this.End;
                var i16OtherStart = ( Int16 ) ( Object ) other.Start;
                var i16OtherEnd = ( Int16 ) ( Object ) other.End;
                return i16Start <= i16OtherEnd && i16OtherStart <= i16End;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16Start = ( UInt16 ) ( Object ) this.Start;
                var u16End = ( UInt16 ) ( Object ) this.End;
                var u16OtherStart = ( UInt16 ) ( Object ) other.Start;
                var u16OtherEnd = ( UInt16 ) ( Object ) other.End;
                return u16Start <= u16OtherEnd && u16OtherStart <= u16End;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32Start = ( Int32 ) ( Object ) this.Start;
                var i32End = ( Int32 ) ( Object ) this.End;
                var i32OtherStart = ( Int32 ) ( Object ) other.Start;
                var i32OtherEnd = ( Int32 ) ( Object ) other.End;
                return i32Start <= i32OtherEnd && i32OtherStart <= i32End;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32Start = ( UInt32 ) ( Object ) this.Start;
                var u32End = ( UInt32 ) ( Object ) this.End;
                var u32OtherStart = ( UInt32 ) ( Object ) other.Start;
                var u32OtherEnd = ( UInt32 ) ( Object ) other.End;
                return u32Start <= u32OtherEnd && u32OtherStart <= u32End;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64Start = ( Int64 ) ( Object ) this.Start;
                var i64End = ( Int64 ) ( Object ) this.End;
                var i64OtherStart = ( Int64 ) ( Object ) other.Start;
                var i64OtherEnd = ( Int64 ) ( Object ) other.End;
                return i64Start <= i64OtherEnd && i64OtherStart <= i64End;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64Start = ( UInt64 ) ( Object ) this.Start;
                var u64End = ( UInt64 ) ( Object ) this.End;
                var u64OtherStart = ( UInt64 ) ( Object ) other.Start;
                var u64OtherEnd = ( UInt64 ) ( Object ) other.End;
                return u64Start <= u64OtherEnd && u64OtherStart <= u64End;
            }
            else
            {
                return this.Start.CompareTo ( other.End ) < 1 && other.Start.CompareTo ( this.End ) < 1;
            }
        }

        /// <summary>
        /// Joins this <see cref="Range{T}" /> with another <see cref="Range{T}" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Range<T> GetUnion ( Range<T> other ) =>
            new ( Min ( this.Start, other.Start ), Max ( this.End, other.End ) );

        /// <summary>
        /// Intersects this <see cref="Range{T}"/> with another <see cref="Range{T}"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Range<T> GetIntersection ( Range<T> other ) =>
            new ( Max ( this.Start, other.Start ), Min ( this.End, other.End ) );

        /// <summary>
        /// Returns whether a certain <paramref name="value" /> is contained inside this <see
        /// cref="Range{T}" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public Boolean ValueIn ( T value )
        {
            if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8Value = ( Byte ) ( Object ) value;
                var u8Start = ( Byte ) ( Object ) this.Start;
                var u8End = ( Byte ) ( Object ) this.End;
                return ( UInt16 ) ( u8Value - u8Start ) <= ( u8End - u8Start );
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16Value = ( UInt16 ) ( Object ) value;
                var u16Start = ( UInt16 ) ( Object ) this.Start;
                var u16End = ( UInt16 ) ( Object ) this.End;
                return ( UInt32 ) ( u16Value - u16Start ) <= ( u16End - u16Start );
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charValue = ( Char ) ( Object ) value;
                var charStart = ( Char ) ( Object ) this.Start;
                var charEnd = ( Char ) ( Object ) this.End;
                return ( UInt32 ) ( charValue - charStart ) <= ( charEnd - charStart );
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32Value = ( UInt32 ) ( Object ) value;
                var u32Start = ( UInt32 ) ( Object ) this.Start;
                var u32End = ( UInt32 ) ( Object ) this.End;
                return ( UInt64 ) ( u32Value - u32Start ) <= ( u32End - u32Start );
            }
            else
            {
                return this.Start.CompareTo ( value ) < 1 && value.CompareTo ( this.End ) < 1;
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

        /// <summary>
        /// Checks whether two ranges are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( Range<T> left, Range<T> right ) => left.Equals ( right );

        /// <summary>
        /// Checks whether two ranges are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Range<T> left, Range<T> right ) => !( left == right );

        /// <summary>
        /// Checks whether one range is located before the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator < ( Range<T> left, Range<T> right ) => left.CompareTo ( right ) < 0;

        /// <summary>
        /// Checks whether one range is located before or at the same position as the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator <= ( Range<T> left, Range<T> right ) => left.CompareTo ( right ) <= 0;

        /// <summary>
        /// Checks whether one range is located after the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator > ( Range<T> left, Range<T> right ) => left.CompareTo ( right ) > 0;

        /// <summary>
        /// Checks whether one range is located after or at the same point as another.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator >= ( Range<T> left, Range<T> right ) => left.CompareTo ( right ) >= 0;

        #endregion Generated Code

#if HAS_VALUETUPLE
        /// <summary>
        /// Converts a tuple into a range.
        /// </summary>
        /// <param name="range"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "The constructor can be used instead." )]
        public static implicit operator Range<T> ( (T start, T end) range ) =>
            new ( range.start, range.end );
#endif

        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static T Min ( T x, T y )
        {
            if ( typeof ( T ) == typeof ( SByte ) )
            {
                var i8x = ( SByte ) ( Object ) x;
                var i8y = ( SByte ) ( Object ) y;
                return i8x <= i8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8x = ( Byte ) ( Object ) x;
                var u8y = ( Byte ) ( Object ) y;
                return u8x <= u8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charx = ( Char ) ( Object ) x;
                var chary = ( Char ) ( Object ) y;
                return charx <= chary ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16x = ( Int16 ) ( Object ) x;
                var i16y = ( Int16 ) ( Object ) y;
                return i16x <= i16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16x = ( UInt16 ) ( Object ) x;
                var u16y = ( UInt16 ) ( Object ) y;
                return u16x <= u16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32x = ( Int32 ) ( Object ) x;
                var i32y = ( Int32 ) ( Object ) y;
                return i32x <= i32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32x = ( UInt32 ) ( Object ) x;
                var u32y = ( UInt32 ) ( Object ) y;
                return u32x <= u32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64x = ( Int64 ) ( Object ) x;
                var i64y = ( Int64 ) ( Object ) y;
                return i64x <= i64y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64x = ( UInt64 ) ( Object ) x;
                var u64y = ( UInt64 ) ( Object ) y;
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
                var i8x = ( SByte ) ( Object ) x;
                var i8y = ( SByte ) ( Object ) y;
                return i8x >= i8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Byte ) )
            {
                var u8x = ( Byte ) ( Object ) x;
                var u8y = ( Byte ) ( Object ) y;
                return u8x >= u8y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Char ) )
            {
                var charx = ( Char ) ( Object ) x;
                var chary = ( Char ) ( Object ) y;
                return charx >= chary ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int16 ) )
            {
                var i16x = ( Int16 ) ( Object ) x;
                var i16y = ( Int16 ) ( Object ) y;
                return i16x >= i16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt16 ) )
            {
                var u16x = ( UInt16 ) ( Object ) x;
                var u16y = ( UInt16 ) ( Object ) y;
                return u16x >= u16y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int32 ) )
            {
                var i32x = ( Int32 ) ( Object ) x;
                var i32y = ( Int32 ) ( Object ) y;
                return i32x >= i32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt32 ) )
            {
                var u32x = ( UInt32 ) ( Object ) x;
                var u32y = ( UInt32 ) ( Object ) y;
                return u32x >= u32y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( Int64 ) )
            {
                var i64x = ( Int64 ) ( Object ) x;
                var i64y = ( Int64 ) ( Object ) y;
                return i64x >= i64y ? x : y;
            }
            else if ( typeof ( T ) == typeof ( UInt64 ) )
            {
                var u64x = ( UInt64 ) ( Object ) x;
                var u64y = ( UInt64 ) ( Object ) y;
                return u64x >= u64y ? x : y;
            }
            else
            {
                return x.CompareTo ( y ) >= 0 ? x : y;
            }
        }
    }
}