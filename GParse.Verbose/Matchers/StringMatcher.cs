using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class StringMatcher : BaseMatcher
    {
        internal readonly String Filter;

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "message", nameof ( filter ) );
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var ismatch = reader.IsNext ( this.Filter, offset );
            length = ismatch ? this.Filter.Length : 0;
            return ismatch;
        }
    }
}
