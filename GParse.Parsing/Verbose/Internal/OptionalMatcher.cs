using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class OptionalMatcher : BaseMatcher
    {
        internal IPatternMatcher PatternMatcher;

        public OptionalMatcher ( IPatternMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader ) => true;

        public override String Match ( SourceCodeReader reader )
        {
            return this.PatternMatcher.IsMatch ( reader ) ? this.PatternMatcher.Match ( reader ) : "";
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
