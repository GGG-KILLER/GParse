using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class MultiCharMatcher : BaseMatcher
    {
        internal readonly Char[] Whitelist;

        public MultiCharMatcher ( params Char[] whitelist )
        {
            this.Whitelist = whitelist;
            Array.Sort ( this.Whitelist );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var ismatch = !reader.EOF ( ) && Array.BinarySearch ( this.Whitelist, ( Char ) reader.Peek ( offset ) ) != -1;
            length = ismatch ? 1 : 0;
            return ismatch;
        }
    }
}
