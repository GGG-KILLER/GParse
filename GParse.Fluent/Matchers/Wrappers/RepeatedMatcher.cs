using System;
using System.Collections.Generic;
using GParse.Common.Math;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches the child matchers a certain amount of times
    /// </summary>
    public sealed class RepeatedMatcher : MatcherWrapper, IEquatable<RepeatedMatcher>
    {
        /// <summary>
        /// The amount of matches that makes the match successful
        /// </summary>
        public readonly Range<UInt32> Range;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="range"></param>
        public RepeatedMatcher ( BaseMatcher matcher, in Range<UInt32> range ) : base ( matcher )
        {
            this.Range = range;
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as RepeatedMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( RepeatedMatcher other ) => other != null && this.Range.Equals ( other.Range ) && base.Equals ( other );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1678107930;
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => EqualityComparer<RepeatedMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( RepeatedMatcher matcher1, RepeatedMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
