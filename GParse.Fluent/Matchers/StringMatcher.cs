using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches a string of characters
    /// </summary>
    public sealed class StringMatcher : BaseMatcher, IEquatable<StringMatcher>, IStringContainerMatcher
    {
        /// <summary>
        /// The string it matches
        /// </summary>
        public String StringFilter { get; }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="filter"></param>
        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "Provided filter must be a non-null, non-empty string", nameof ( filter ) );
            this.StringFilter = filter;
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as StringMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( StringMatcher other ) => other != null &&
                     this.StringFilter == other.StringFilter;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 433820461;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.StringFilter );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( StringMatcher matcher1, StringMatcher matcher2 ) => EqualityComparer<StringMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( StringMatcher matcher1, StringMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
