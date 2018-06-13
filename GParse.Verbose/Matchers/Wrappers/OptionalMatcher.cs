using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class OptionalMatcher : MatcherWrapper
    {
        public OptionalMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
        {
            var len = base.MatchLength ( reader, offset );
            return len != -1 ? len : 0;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return base.MatchLength ( reader ) != -1
                ? base.Match ( reader )
                : Array.Empty<String> ( );
        }

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is OptionalMatcher
                && base.Equals ( obj );
        }
    }
}
