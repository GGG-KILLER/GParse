using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class NegatedMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;

        public NegatedMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = 0;
            return !this.PatternMatcher.IsMatch ( reader, out var _, offset );
        }
    }
}
