using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public sealed class AnyMatcher : BaseMatcher, IEquatable<AnyMatcher>
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AnyMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as AnyMatcher );
        }

        public Boolean Equals ( AnyMatcher other )
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

        public static Boolean operator == ( AnyMatcher matcher1, AnyMatcher matcher2 ) => EqualityComparer<AnyMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( AnyMatcher matcher1, AnyMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
