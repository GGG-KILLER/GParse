using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class StringMatcher : BaseMatcher
    {
        private readonly String Filter;

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "message", nameof ( filter ) );
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return reader.IsNext ( this.Filter );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;
            reader.Advance ( this.Filter.Length );
            return this.Filter;
        }

        public override void ResetInternalState ( )
        {
            // Noop
        }
    }
}
