using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public sealed class AllMatcher : BaseMatcher, IEquatable<AllMatcher>
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AllMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as AllMatcher );
        }

        public Boolean Equals ( AllMatcher other )
        {
            return other != null &&
                    EqualityComparer<BaseMatcher[]>.Default.Equals ( this.PatternMatchers, other.PatternMatchers );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 928612024;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher[]>.Default.GetHashCode ( this.PatternMatchers );
            return hashCode;
        }

        public static Boolean operator == ( AllMatcher matcher1, AllMatcher matcher2 ) => EqualityComparer<AllMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( AllMatcher matcher1, AllMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
