using System;

namespace GParse.Verbose.MathUtils
{
    /// <summary>
    /// An inclusive <see cref="UInt32" /> inclusive range
    /// </summary>
    public struct Range : IEquatable<Range>
    {
        /// <summary>
        /// Starting location of the range
        /// </summary>
        public readonly UInt32 Start;

        /// <summary>
        /// Ending location of the range (inclusive)
        /// </summary>
        public readonly UInt32 End;

        /// <summary>
        /// Whether this range spans a single element
        /// </summary>
        public readonly Boolean IsSingle;

        /// <summary>
        /// Inner <see cref="ToArray"/> memoization
        /// </summary>
        private UInt32[] AsArray;

        /// <summary>
        /// Initializes a range that spans a single number
        /// </summary>
        /// <param name="number"></param>
        public Range ( UInt32 number )
        {
            this.Start = number;
            this.End = number;
            this.IsSingle = true;
            this.AsArray = null;
        }

        /// <summary>
        /// Initializes a range with a start and end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Range ( UInt32 start, UInt32 end )
        {
            if ( end < start )
                throw new InvalidOperationException ( "Cannot initialize a range with a start greater than the end" );

            this.Start = start;
            this.End = end;
            this.IsSingle = Math.Abs ( end - start ) == 0;
            this.AsArray = null;
        }

        /// <summary>
        /// Returns whether this <see cref="Range" /> intersects
        /// with another <see cref="Range" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean IntersectsWith ( Range other )
        {
            return ( this.Start <= other.Start && other.Start <= this.End )
                || ( this.Start <= other.End && other.End <= this.End );
        }

        /// <summary>
        /// Returns whether this <see cref="Range"/> is a neighbour to another <see cref="Range"/> (e.g.: [1, 2] and [3, 4])
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean IsNeighbourOf ( Range other )
        {
            return Math.Abs ( this.End - other.Start ) == 1 || Math.Abs ( this.Start - other.End ) == 1;
        }

        /// <summary>
        /// Joins this <see cref="Range" /> with another <see cref="Range" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Range JoinWith ( Range other )
        {
            return this.IntersectsWith ( other ) || this.IsNeighbourOf ( other )
                ? new Range ( Math.Min ( this.Start, other.Start ), Math.Max ( this.End, other.End ) )
                : throw new InvalidOperationException ( "Cannot join two ranges that do not intersect" );
        }

        /// <summary>
        /// Returns whether a certain <paramref name="value" /> is
        /// contained inside this <see cref="Range" />
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean ValueIn ( UInt32 value )
            => this.Start <= value && value <= this.End;

        /// <summary>
        /// Returns all elements contained in the range as an array
        /// </summary>
        /// <returns></returns>
        public UInt32[] ToArray ( )
        {
            if ( this.AsArray == null )
            {
                var delta = this.End - this.Start;
                this.AsArray = new UInt32[delta];
                for ( var i = 0U; i < delta; i++ )
                    this.AsArray[i] = this.Start + i;
            }
            return this.AsArray;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return obj is Range && this.Equals ( ( Range ) obj );
        }

        public Boolean Equals ( Range other )
        {
            return this.Start == other.Start &&
                     this.End == other.End;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + this.Start.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.End.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( Range range1, Range range2 ) => range1.Equals ( range2 );

        public static Boolean operator != ( Range range1, Range range2 ) => !( range1 == range2 );

        #endregion Generated Code
    }
}
