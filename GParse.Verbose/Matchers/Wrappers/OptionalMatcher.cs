using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public sealed class OptionalMatcher : MatcherWrapper
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
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
