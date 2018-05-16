using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class FilterFuncMatcher : BaseMatcher
    {
        internal Func<Char, Boolean> Filter;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var res = !reader.EOF ( ) && this.Filter ( ( Char ) reader.Peek ( offset ) );
            length = res ? 1 : 0;
            return res;
        }
    }
}
