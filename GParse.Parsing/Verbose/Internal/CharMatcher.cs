using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct CharMatcher : IPatternMatcher
    {
        private readonly Char Filter;

        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            return this.Filter == reader.Peek ( );
        }

        public String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        public void ResetInternalState ( )
        {
            // noop
        }
    }
}
