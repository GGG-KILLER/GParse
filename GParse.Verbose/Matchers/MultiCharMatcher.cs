using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class MultiCharMatcher : BaseMatcher, IEquatable<MultiCharMatcher>
    {
        internal readonly Char[] Whitelist;

        /// <summary>
        /// Make sure that for a better performance, the most
        /// commonly matched characters are listed first in the whitelist.
        /// </summary>
        /// <param name="whitelist"></param>
        public MultiCharMatcher ( params Char[] whitelist )
        {
            if ( whitelist.Length < 2 )
                throw new ArgumentException ( "Whitelist should contain at least 2 elements.", nameof ( whitelist ) );
            this.Whitelist = whitelist;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as MultiCharMatcher );
        }

        public Boolean Equals ( MultiCharMatcher other )
        {
            return other != null &&
                    EqualityComparer<Char[]>.Default.Equals ( this.Whitelist, other.Whitelist );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -187114584;
            hashCode = hashCode * -1521134295 + EqualityComparer<Char[]>.Default.GetHashCode ( this.Whitelist );
            return hashCode;
        }

        public static Boolean operator == ( MultiCharMatcher matcher1, MultiCharMatcher matcher2 ) => EqualityComparer<MultiCharMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( MultiCharMatcher matcher1, MultiCharMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
