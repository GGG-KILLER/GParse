using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public class MarkerMatcher : MatcherWrapper
    {
        public MarkerMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );
    }
}
