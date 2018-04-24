using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct MultiCharMatcher : IPatternMatcher
    {
        internal readonly Char[] Whitelist;

        public MultiCharMatcher ( params Char[] whitelist )
        {
            this.Whitelist = whitelist;
            Array.Sort ( this.Whitelist );
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && Array.BinarySearch ( this.Whitelist, ( Char ) reader.Peek ( ) ) != -1;
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
