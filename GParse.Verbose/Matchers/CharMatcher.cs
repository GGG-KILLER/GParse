using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class CharMatcher : BaseMatcher
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
    }
}
