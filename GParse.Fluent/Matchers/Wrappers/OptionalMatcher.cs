using System;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    public sealed class OptionalMatcher : MatcherWrapper
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public OptionalMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean Equals ( Object obj )
        {
            return obj != null
                && obj is OptionalMatcher
                && base.Equals ( obj );
        }
    }
}
