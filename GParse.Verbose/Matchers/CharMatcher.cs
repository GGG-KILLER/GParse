using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class CharMatcher : BaseMatcher
    {
        internal readonly Char Filter;

        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var res = this.Filter == reader.Peek ( offset );
            length = res ? 1 : 0;
            return res;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader, out var _ ) )
            {
                reader.Advance ( 1 );
                return new[] { this.Filter.ToString ( ) };
            }
            throw new ParseException ( reader.Location, $"Expected '{this.Filter}' but got '{( Char ) reader.Peek ( )}'." );
        }
    }
}
