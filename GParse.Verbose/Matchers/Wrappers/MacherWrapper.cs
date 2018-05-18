using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public abstract class MatcherWrapper : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;

        protected MatcherWrapper ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            return this.PatternMatcher.IsMatch ( reader, out length, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.PatternMatcher.Match ( reader );
        }

        // Reset the internal state of the contained matcher by default
        public override void ResetInternalState ( )
        {
            this.PatternMatcher.ResetInternalState ( );
        }
    }
}
