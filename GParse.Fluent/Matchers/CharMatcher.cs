using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches a single char
    /// </summary>
    public sealed class CharMatcher : BaseMatcher, IEquatable<CharMatcher>, IStringContainerMatcher
    {
        /// <summary>
        /// The char it matches
        /// </summary>
        public readonly Char Filter;

        /// <summary>
        /// The char it matches as a string
        /// </summary>
        public String StringFilter { get; }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="filter"></param>
        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
            this.StringFilter = filter.ToString ( );
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as CharMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( CharMatcher other ) => other != null &&
                     this.Filter == other.Filter;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1419471041;
            hashCode = hashCode * -1521134295 + this.Filter.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( CharMatcher matcher1, CharMatcher matcher2 ) => EqualityComparer<CharMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( CharMatcher matcher1, CharMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
