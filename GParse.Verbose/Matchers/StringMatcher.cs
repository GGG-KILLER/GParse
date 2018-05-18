using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class StringMatcher : BaseMatcher
    {
        internal readonly String Filter;

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "Provided filter must be a non-null, non-empty string", nameof ( filter ) );
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var ismatch = !reader.EOF ( ) && reader.IsNext ( this.Filter, offset );
            length = ismatch ? this.Filter.Length : 0;
            return ismatch;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader, out var len ) )
            {
                reader.Advance ( len );
                return new[] { this.Filter };
            }

            throw new ParseException ( reader.Location, $"Expected \"{this.Filter}\" but got \"{reader.PeekString ( this.Filter.Length )}\"" );
        }
    }
}
