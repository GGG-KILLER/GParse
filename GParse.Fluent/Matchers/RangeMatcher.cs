using System;
using System.Collections.Generic;
using GParse.Math;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches characters inside a range
    /// </summary>
    public sealed class RangeMatcher : BaseMatcher, IEquatable<RangeMatcher>
    {
        /// <summary>
        /// The range which it checks against
        /// </summary>
        public readonly Range<Char> Range;

        /// <summary>
        /// </summary>
        /// <param name="range">The range itself</param>
        public RangeMatcher ( in Range<Char> range )
        {
            this.Range = range;
        }

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        public RangeMatcher ( in Char Start, in Char End ) : this ( new Range<Char> ( Start, End ) )
        {
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as RangeMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( RangeMatcher other ) => other != null && this.Range.Equals ( other.Range );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -2061379221;
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( RangeMatcher matcher1, RangeMatcher matcher2 ) => EqualityComparer<RangeMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( RangeMatcher matcher1, RangeMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
