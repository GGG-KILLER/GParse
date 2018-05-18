using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class JoinMatcher : MatcherWrapper
    {
        public JoinMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override String[] Match ( SourceCodeReader reader )
            => new[] { String.Join ( "", base.Match ( reader ) ) };
    }
}
