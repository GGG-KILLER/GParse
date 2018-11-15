using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches one of the characters
    /// </summary>
    public sealed class CharListMatcher : BaseMatcher, IEquatable<CharListMatcher>
    {
        /// <summary>
        /// The whitelist to check against
        /// </summary>
        public readonly Char[] Whitelist;

        /// <summary>
        /// Initializes tihs matcher
        /// </summary>
        /// <param name="whitelist">Make sure that for a better performance, the most
        /// commonly matched characters are listed first.</param>
        public CharListMatcher ( params Char[] whitelist )
        {
            if ( whitelist.Length < 2 )
                throw new ArgumentException ( "Whitelist should contain at least 2 elements.", nameof ( whitelist ) );
            this.Whitelist = whitelist;
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as CharListMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( CharListMatcher other ) => other != null &&
                    EqualityComparer<Char[]>.Default.Equals ( this.Whitelist, other.Whitelist );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -187114584;
            hashCode = hashCode * -1521134295 + EqualityComparer<Char[]>.Default.GetHashCode ( this.Whitelist );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( CharListMatcher matcher1, CharListMatcher matcher2 ) => EqualityComparer<CharListMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( CharListMatcher matcher1, CharListMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
