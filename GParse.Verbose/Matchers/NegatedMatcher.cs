using System;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class NegatedMatcher : BaseMatcher
    {
        internal IPatternMatcher PatternMatcher;

        public NegatedMatcher ( IPatternMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return !this.PatternMatcher.IsMatch ( reader );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? "" : null;
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
