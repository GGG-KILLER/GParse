using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches a sequence of matchers
    /// </summary>
    public sealed class SequentialMatcher : BaseMatcher, IEquatable<SequentialMatcher>
    {
        /// <summary>
        /// The matchers it matches
        /// </summary>
        public readonly BaseMatcher[] PatternMatchers;

        /// <summary>
        /// Initializes this
        /// </summary>
        /// <param name="patternMatchers"></param>
        public SequentialMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as SequentialMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( SequentialMatcher other )
        {
            if ( other == null || other.PatternMatchers.Length != this.PatternMatchers.Length )
                return false;

            for ( var i = 0; i < this.PatternMatchers.Length; i++ )
                if ( !this.PatternMatchers[i].Equals ( other.PatternMatchers[i] ) )
                    return false;
            return true;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 1903173070;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher[]>.Default.GetHashCode ( this.PatternMatchers );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( SequentialMatcher matcher1, SequentialMatcher matcher2 ) => EqualityComparer<SequentialMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( SequentialMatcher matcher1, SequentialMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
