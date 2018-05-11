using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class OnceMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;
        private Boolean Matched;

        public OnceMatcher ( BaseMatcher patternMatcher )
        {
            this.PatternMatcher = patternMatcher;
            this.Matched = false;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 len, Int32 offset = 0 )
        {
            if ( this.Matched )
            {
                len = 0;
                return false;
            }
            return this.PatternMatcher.IsMatch ( reader, out len, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
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
