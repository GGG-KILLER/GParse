using System;

namespace GParse.Verbose.Matchers
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    public sealed class JoinMatcher : MatcherWrapper
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public JoinMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is JoinMatcher
                && base.Equals ( obj );
        }
    }
}
