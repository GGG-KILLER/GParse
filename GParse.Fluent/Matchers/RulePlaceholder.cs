using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// A placeholder for a rule
    /// </summary>
    public sealed class RulePlaceholder : BaseMatcher, IEquatable<RulePlaceholder>
    {
        /// <summary>
        /// The name of the rule
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="Name"></param>
        public RulePlaceholder ( String Name )
        {
            this.Name = Name;
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

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as RulePlaceholder );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( RulePlaceholder other ) => other != null &&
                     this.Name == other.Name;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1940561674;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="placeholder1"></param>
        /// <param name="placeholder2"></param>
        /// <returns></returns>
        public static Boolean operator == ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => EqualityComparer<RulePlaceholder>.Default.Equals ( placeholder1, placeholder2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="placeholder1"></param>
        /// <param name="placeholder2"></param>
        /// <returns></returns>
        public static Boolean operator != ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => !( placeholder1 == placeholder2 );

        #endregion Generated Code
    }
}
