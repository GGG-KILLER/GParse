using System;
using GParse.Fluent.Abstractions;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Ignores the contents of its child matcher
    /// </summary>
    public sealed class IgnoreMatcher : MatcherWrapper
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="matcher"></param>
        public IgnoreMatcher ( BaseMatcher matcher ) : base ( matcher )
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
        public override Boolean Equals ( Object obj ) => obj != null && obj is IgnoreMatcher && base.Equals ( obj );
    }
}
