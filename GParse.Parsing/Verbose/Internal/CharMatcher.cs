using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class CharMatcher : BaseMatcher
    {
        private readonly Char Filter;

        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return this.Filter == reader.Peek ( );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
