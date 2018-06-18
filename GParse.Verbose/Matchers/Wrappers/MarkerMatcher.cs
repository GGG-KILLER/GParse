using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public class MarkerMatcher : MatcherWrapper
    {
        internal readonly VerboseParser Parser;

        public MarkerMatcher ( VerboseParser parser, BaseMatcher matcher ) : base ( matcher )
        {
            this.Parser = parser;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var match = base.Match ( reader );
            this.Parser.MarkerMatcherMatched ( match );
            return match;
        }
    }
}
