using System;
using GParse.Common.IO;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
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

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is IgnoreMatcher
                && base.Equals ( obj );
        }
    }
}
