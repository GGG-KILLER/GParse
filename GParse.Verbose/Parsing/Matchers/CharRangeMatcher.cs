using System;
using System.Collections.Generic;
using GParse.Verbose.MathUtils;
using GParse.Verbose.Parsing.Abstractions;

namespace GParse.Verbose.Parsing.Matchers
{
    public sealed class CharRangeMatcher : BaseMatcher, IEquatable<CharRangeMatcher>
    {
        public readonly Range Range;

        /// <summary>
        /// </summary>
        /// <param name="range">The range itself</param>
        public CharRangeMatcher ( in Range range )
        {
            this.Range = range;
        }

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        public CharRangeMatcher ( in Char Start, in Char End )
        {
            this.Range = new Range ( Start, End );
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
            return other != null && this.Range.Equals ( other.Range );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -2061379221;
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => EqualityComparer<CharRangeMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
