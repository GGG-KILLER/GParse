using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class CharMatcher : BaseMatcher, IEquatable<CharMatcher>, IStringContainerMatcher
    {
        internal readonly Char Filter;
        public String StringFilter { get; }

        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
            this.StringFilter = filter.ToString ( );
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as CharMatcher );
        }

        public Boolean Equals ( CharMatcher other )
        {
            return other != null &&
                     this.Filter == other.Filter &&
                     this.StringFilter == other.StringFilter;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1419471041;
            hashCode = hashCode * -1521134295 + this.Filter.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.StringFilter );
            return hashCode;
        }

        public static Boolean operator == ( CharMatcher matcher1, CharMatcher matcher2 ) => EqualityComparer<CharMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( CharMatcher matcher1, CharMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
