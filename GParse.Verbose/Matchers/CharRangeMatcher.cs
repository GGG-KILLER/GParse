using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class CharRangeMatcher : BaseMatcher, IEquatable<CharRangeMatcher>
    {
        public readonly Char Start;
        public readonly Char End;

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        public CharRangeMatcher ( Char Start, Char End, Boolean rawValues = false )
        {
            this.Start = ( Char ) Math.Min ( Start, End );
            this.End = ( Char ) Math.Max ( Start, End );
            if ( !rawValues )
            {
                this.Start--;
                this.End++;
            }
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as CharRangeMatcher );
        }

        public Boolean Equals ( CharRangeMatcher other )
        {
            return other != null &&
                     this.Start == other.Start &&
                     this.End == other.End;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -2061379221;
            hashCode = hashCode * -1521134295 + this.Start.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.End.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => EqualityComparer<CharRangeMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
