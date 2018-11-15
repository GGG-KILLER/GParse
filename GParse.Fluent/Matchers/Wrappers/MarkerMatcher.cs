using GParse.Fluent.Abstractions;
using GParse.Fluent.AST;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Emits the contents matched by child matchers as a <see cref="MarkerNode"/>
    /// </summary>
    public class MarkerMatcher : MatcherWrapper
    {
        /// <summary>
        /// Initialies this class
        /// </summary>
        /// <param name="matcher"></param>
        public MarkerMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );
    }
}
