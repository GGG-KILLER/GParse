using System;
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

        public override String[] Match ( SourceCodeReader reader )
        {
            var m = this.PatternMatcher.Match ( reader );
            return m ?? Array.Empty<String> ( );
        }

        public override void ResetInternalState ( )
        {
            this.PatternMatcher.ResetInternalState ( );
        }
    }
}
