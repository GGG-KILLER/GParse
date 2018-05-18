using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class IgnoreMatcher : MatcherWrapper
    {
        public IgnoreMatcher ( BaseMatcher matcher ) : base (matcher)
        {
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            base.Match ( reader );
            return Array.Empty<String> ( );
        }
    }
}
