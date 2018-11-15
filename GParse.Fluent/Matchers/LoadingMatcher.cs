using System;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches a previously stored value
    /// </summary>
    public class LoadingMatcher : BaseMatcher
    {
        /// <summary>
        /// The name of the saved value
        /// </summary>
        public readonly String SaveName;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="saveName"></param>
        public LoadingMatcher ( String saveName )
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
