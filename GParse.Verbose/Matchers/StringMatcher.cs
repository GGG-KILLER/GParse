using System;
using System.Collections.Generic;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class StringMatcher : BaseMatcher, IEquatable<StringMatcher>
    {
        internal readonly String Filter;

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "Provided filter must be a non-null, non-empty string", nameof ( filter ) );
            this.Filter = filter;
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
            => !reader.EOF ( ) && reader.IsNext ( this.Filter, offset )
                ? this.Filter.Length
                : -1;

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( this.MatchLength ( reader ) != -1 )
            {
                reader.Advance ( this.Filter.Length );
                return new[] { this.Filter };
            }

            throw new ParseException ( reader.Location, $"Expected \"{this.Filter}\" but got \"{reader.PeekString ( this.Filter.Length )}\"" );
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as StringMatcher );
        }

        public Boolean Equals ( StringMatcher other )
        {
            return other != null &&
                     this.Filter == other.Filter;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 433820461;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Filter );
            return hashCode;
        }

        public static Boolean operator == ( StringMatcher matcher1, StringMatcher matcher2 ) => EqualityComparer<StringMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( StringMatcher matcher1, StringMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
