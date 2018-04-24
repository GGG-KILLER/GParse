using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class MultiCharMatcher : BaseMatcher
    {
        internal readonly Char[] Whitelist;

        public MultiCharMatcher ( params Char[] whitelist )
        {
            this.Whitelist = whitelist;
            Array.Sort ( this.Whitelist );
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && Array.BinarySearch ( this.Whitelist, ( Char ) reader.Peek ( ) ) != -1;
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
