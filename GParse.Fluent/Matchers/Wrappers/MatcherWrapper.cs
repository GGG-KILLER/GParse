using System;
using System.Collections.Generic;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Base class for matchers that wrap other matchers
    /// </summary>
    public abstract class MatcherWrapper : BaseMatcher, IEquatable<MatcherWrapper>
    {
        /// <summary>
        /// The matcher this matcher wraps
        /// </summary>
        public readonly BaseMatcher PatternMatcher;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="matcher"></param>
        protected MatcherWrapper ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as MatcherWrapper );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( MatcherWrapper other ) => other != null && EqualityComparer<BaseMatcher>.Default.Equals ( this.PatternMatcher, other.PatternMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1683483781;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher>.Default.GetHashCode ( this.PatternMatcher );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="wrapper1"></param>
        /// <param name="wrapper2"></param>
        /// <returns></returns>
        public static Boolean operator == ( MatcherWrapper wrapper1, MatcherWrapper wrapper2 ) => EqualityComparer<MatcherWrapper>.Default.Equals ( wrapper1, wrapper2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="wrapper1"></param>
        /// <param name="wrapper2"></param>
        /// <returns></returns>
        public static Boolean operator != ( MatcherWrapper wrapper1, MatcherWrapper wrapper2 ) => !( wrapper1 == wrapper2 );

        #endregion Generated Code
    }
}
