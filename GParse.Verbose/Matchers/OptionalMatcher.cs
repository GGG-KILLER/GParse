using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class OptionalMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;

        public OptionalMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            if ( !this.PatternMatcher.IsMatch ( reader, out length, offset ) )
                length = 0;
            return true;
        }
    }
}
