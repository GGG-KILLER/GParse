using System;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    /// <summary>
    /// Makes matching of the child matchers optional
    /// </summary>
    public sealed class OptionalMatcher : MatcherWrapper
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="matcher"></param>
        public OptionalMatcher ( BaseMatcher matcher ) : base ( matcher )
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj != null && obj is OptionalMatcher && base.Equals ( obj );
    }
}
