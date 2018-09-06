using System;
using System.Collections.Generic;
using GParse.Common.Math;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public sealed class RepeatedMatcher : MatcherWrapper, IEquatable<RepeatedMatcher>
    {
        /// <summary>
        /// Maximum count of matches to be successful
        /// </summary>
        public readonly Range<UInt32> Range;

        public RepeatedMatcher ( BaseMatcher matcher, in Range<UInt32> range ) : base ( matcher )
        {
            this.Range = range;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as RepeatedMatcher );
        }

        public Boolean Equals ( RepeatedMatcher other )
        {
            return other != null && this.Range.Equals ( other.Range );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1678107930;
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => EqualityComparer<RepeatedMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
