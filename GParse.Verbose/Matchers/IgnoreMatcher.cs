using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public class IgnoreMatcher : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;

        public IgnoreMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            return this.PatternMatcher.IsMatch ( reader, out length, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var m = this.PatternMatcher.Match ( reader );
            return m == null ? null : Array.Empty<String> ( );
        }

        public override void ResetInternalState ( )
        {
            this.PatternMatcher.ResetInternalState ( );
        }
    }
}
