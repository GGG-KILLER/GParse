using System;
using GParse.Fluent.Abstractions;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace GParse.Fluent.Matchers
{
    public sealed class NegatedMatcher : MatcherWrapper
    {
        public NegatedMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is NegatedMatcher
                && base.Equals ( obj );
        }
    }
}
