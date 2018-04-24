using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class OnceMatcher : BaseMatcher
    {
        internal IPatternMatcher PatternMatcher;
        private Boolean Matched;

        public OnceMatcher ( IPatternMatcher patternMatcher )
        {
            this.PatternMatcher = patternMatcher;
            this.Matched = false;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return this.Matched && this.PatternMatcher.IsMatch ( reader );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.Matched )
            {
                this.Matched = true;
                return this.PatternMatcher.Match ( reader );
            }
            return null;
        }

        public override void ResetInternalState ( )
        {
            this.Matched = false;
        }
    }
}
