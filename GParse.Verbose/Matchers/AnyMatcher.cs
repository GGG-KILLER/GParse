using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class AnyMatcher : BaseMatcher, IEquatable<AnyMatcher>
    {
        public readonly BaseMatcher[] PatternMatchers;

        public AnyMatcher ( params BaseMatcher[] patternMatchers )
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
            return this.Equals ( obj as AnyMatcher );
        }

        public Boolean Equals ( AnyMatcher other )
        {
            if ( other == null || other.PatternMatchers.Length != this.PatternMatchers.Length )
                return false;

            // They don't necessarily need to be in the same order (yes, I know, slow check)
            for ( var i = 0; i < this.PatternMatchers.Length; i++ )
                if ( Array.IndexOf ( other.PatternMatchers, this.PatternMatchers[i] ) == -1 )
                    return false;
            return true;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 172139865;
            foreach ( BaseMatcher matcher in this.PatternMatchers )
                hashCode = hashCode * -1521134295 + matcher.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( AnyMatcher matcher1, AnyMatcher matcher2 ) => EqualityComparer<AnyMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( AnyMatcher matcher1, AnyMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
