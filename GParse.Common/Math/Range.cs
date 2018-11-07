namespace GParse.Common.Math
{
    using System;

    /// <summary>
    /// An inclusive <see cref="UInt32" /> inclusive range
    /// </summary>
    public readonly struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
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
        /// Returns whether this <see cref="Range" /> intersects
        /// with another <see cref="Range" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean IntersectsWith ( Range<T> other ) =>
            this.ValueIn ( other.Start ) || this.ValueIn ( other.End );

        /// <summary>
        /// Joins this <see cref="Range" /> with another <see cref="Range" />
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
        /// Returns whether a certain <paramref name="value" /> is
        /// contained inside this <see cref="Range" />
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean ValueIn ( T value )
            => this.Start.CompareTo ( value ) < 1 && value.CompareTo ( this.End ) < 1;

        #region Generated Code

        public override Boolean Equals ( Object obj ) =>
            obj is Range<T> && this.Equals ( ( Range<T> ) obj );

        public Boolean Equals ( Range<T> other ) =>
            this.Start.CompareTo ( other.Start ) == 0 &&
                     this.End.CompareTo ( other.End ) == 0;

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + this.Start.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.End.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( Range<T> range1, Range<T> range2 ) => range1.Equals ( range2 );

        public static Boolean operator != ( Range<T> range1, Range<T> range2 ) => !( range1 == range2 );

        #endregion Generated Code
    }
}
