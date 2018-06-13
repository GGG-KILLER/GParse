using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class NegatedMatcher : MatcherWrapper
    {
        public NegatedMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return base.MatchLength ( reader, offset ) == -1 ? 0 : -1;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( base.MatchLength ( reader ) == -1 )
                return Array.Empty<String> ( );
            throw new ParseException ( reader.Location, $"Matched expression that wasn't meant to be matched." );
        }

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is NegatedMatcher
                && base.Equals ( obj );
        }
    }
}
