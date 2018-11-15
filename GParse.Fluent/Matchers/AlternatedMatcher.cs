using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Defines a matcher that matches any of the given matchers
    /// </summary>
    public sealed class AlternatedMatcher : BaseMatcher, IEquatable<AlternatedMatcher>
    {
        /// <summary>
        /// The matchers that this matcher will match
        /// </summary>
        public readonly BaseMatcher[] PatternMatchers;

        /// <summary>
        /// Initializes this
        /// </summary>
        /// <param name="patternMatchers"></param>
        public AlternatedMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="alternative"></param>
        /// <returns></returns>
        public override BaseMatcher Or ( BaseMatcher alternative )
        {
            var arr = new BaseMatcher[this.PatternMatchers.Length + 1];
            this.PatternMatchers.CopyTo ( arr, 0 );
            arr[this.PatternMatchers.Length] = alternative;
            return new AlternatedMatcher ( arr );
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as AlternatedMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( AlternatedMatcher other )
        {
            if ( other == null || other.PatternMatchers.Length != this.PatternMatchers.Length )
                return false;

            // They don't necessarily need to be in the same order
            // (yes, I know, slow check)
            for ( var i = 0; i < this.PatternMatchers.Length; i++ )
                if ( Array.IndexOf ( other.PatternMatchers, this.PatternMatchers[i] ) == -1 )
                    return false;
            return true;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 172139865;
            foreach ( BaseMatcher matcher in this.PatternMatchers )
                hashCode = hashCode * -1521134295 + matcher.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( AlternatedMatcher matcher1, AlternatedMatcher matcher2 ) => EqualityComparer<AlternatedMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( AlternatedMatcher matcher1, AlternatedMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
