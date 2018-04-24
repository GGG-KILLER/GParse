using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct FilterFuncMatcher : IPatternMatcher
    {
        internal Func<Char, Boolean> Filter;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && this.Filter ( ( Char ) reader.Peek ( ) );
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
