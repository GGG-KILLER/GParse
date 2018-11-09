using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public sealed class SequentialMatcher : BaseMatcher, IEquatable<SequentialMatcher>
    {
        public readonly BaseMatcher[] PatternMatchers;

        public SequentialMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override BaseMatcher Then ( BaseMatcher matcher )
        {
            // Create a new array with an expanded size
            var arr = new BaseMatcher[this.PatternMatchers.Length + 1];
            // Copy over the old array
            Array.Copy ( this.PatternMatchers, arr, this.PatternMatchers.Length );
            // Then insert the element to be added at the end
            arr[this.PatternMatchers.Length] = matcher;
            return new SequentialMatcher ( arr );
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as SequentialMatcher );

        public Boolean Equals ( SequentialMatcher other )
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

        public static Boolean operator == ( SequentialMatcher matcher1, SequentialMatcher matcher2 ) => EqualityComparer<SequentialMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( SequentialMatcher matcher1, SequentialMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
