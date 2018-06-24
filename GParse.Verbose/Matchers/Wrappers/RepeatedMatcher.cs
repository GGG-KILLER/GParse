using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class RepeatedMatcher : MatcherWrapper, IEquatable<RepeatedMatcher>
    {
        /// <summary>
        /// Maximum count of matches to be successful
        /// </summary>
        internal readonly Int32 Maximum;

        /// <summary>
        /// Minimum amount of matches to be successful
        /// </summary>
        internal readonly Int32 Minimum;

        public RepeatedMatcher ( BaseMatcher matcher, Int32 minimum = 0, Int32 maximum = Int32.MaxValue ) : base ( matcher )
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
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
            return other != null &&
                     this.Maximum == other.Maximum &&
                     this.Minimum == other.Minimum;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1678107930;
            hashCode = hashCode * -1521134295 + this.Maximum.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.Minimum.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => EqualityComparer<RepeatedMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
