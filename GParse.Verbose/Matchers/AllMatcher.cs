using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class AllMatcher : BaseMatcher, IEquatable<AllMatcher>
    {
        public readonly BaseMatcher[] PatternMatchers;

        public AllMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as AllMatcher );
        }

        public Boolean Equals ( AllMatcher other )
        {
            if ( other == null || other.PatternMatchers.Length != this.PatternMatchers.Length )
                return false;

            for ( var i = 0; i < this.PatternMatchers.Length; i++ )
                if ( !this.PatternMatchers[i].Equals ( other.PatternMatchers[i] ) )
                    return false;
            return true;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 1903173070;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher[]>.Default.GetHashCode ( this.PatternMatchers );
            return hashCode;
        }

        public static Boolean operator == ( AllMatcher matcher1, AllMatcher matcher2 ) => EqualityComparer<AllMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( AllMatcher matcher1, AllMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
