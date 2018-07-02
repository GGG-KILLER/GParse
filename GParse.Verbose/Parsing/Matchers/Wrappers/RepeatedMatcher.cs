using System;
using System.Collections.Generic;
using GParse.Verbose.MathUtils;
using GParse.Verbose.Parsing.Abstractions;

namespace GParse.Verbose.Parsing.Matchers
{
    public sealed class RepeatedMatcher : MatcherWrapper, IEquatable<RepeatedMatcher>
    {
        /// <summary>
        /// Maximum count of matches to be successful
        /// </summary>
        public readonly Range Range;

        public RepeatedMatcher ( BaseMatcher matcher, Range? range = null ) : base ( matcher )
        {
            this.Range = range ?? new Range ( 0, UInt32.MaxValue );
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
