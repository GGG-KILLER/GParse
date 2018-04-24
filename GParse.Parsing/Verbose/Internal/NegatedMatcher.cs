using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class NegatedMatcher : BaseMatcher
    {
        internal IPatternMatcher PatternMatcher;

        public NegatedMatcher(IPatternMatcher matcher)
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return !this.PatternMatcher.IsMatch ( reader );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return "";
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
