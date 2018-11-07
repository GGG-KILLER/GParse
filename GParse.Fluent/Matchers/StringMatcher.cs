using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public sealed class StringMatcher : BaseMatcher, IEquatable<StringMatcher>, IStringContainerMatcher
    {
        public String StringFilter { get; }

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "Provided filter must be a non-null, non-empty string", nameof ( filter ) );
            this.StringFilter = filter;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as StringMatcher );

        public Boolean Equals ( StringMatcher other ) => other != null &&
                     this.StringFilter == other.StringFilter;

        public override Int32 GetHashCode ( )
        {
            var hashCode = 433820461;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.StringFilter );
            return hashCode;
        }

        public static Boolean operator == ( StringMatcher matcher1, StringMatcher matcher2 ) => EqualityComparer<StringMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( StringMatcher matcher1, StringMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
