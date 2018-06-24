using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
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
