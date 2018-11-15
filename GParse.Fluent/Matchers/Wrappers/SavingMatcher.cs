using System;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Saves the content matched by child matchers
    /// </summary>
    public class SavingMatcher : MatcherWrapper
    {
        /// <summary>
        /// The key under which to save the contents
        /// </summary>
        public readonly String SaveName;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="matcher"></param>
        public SavingMatcher ( String saveName, BaseMatcher matcher ) : base ( matcher )
        {
            this.SaveName = saveName;
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
