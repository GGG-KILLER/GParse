using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Wraps a rule
    /// </summary>
    public sealed class RuleWrapper : MatcherWrapper, IEquatable<RuleWrapper>
    {
        /// <summary>
        /// The name of the rule
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="Matcher"></param>
        /// <param name="Name"></param>
        public RuleWrapper ( BaseMatcher Matcher, String Name )
            : base ( Matcher )
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as RuleWrapper );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( RuleWrapper other ) => other != null && this.Name == other.Name && base.Equals ( other );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 593797347;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="wrapper1"></param>
        /// <param name="wrapper2"></param>
        /// <returns></returns>
        public static Boolean operator == ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => EqualityComparer<RuleWrapper>.Default.Equals ( wrapper1, wrapper2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="wrapper1"></param>
        /// <param name="wrapper2"></param>
        /// <returns></returns>
        public static Boolean operator != ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => !( wrapper1 == wrapper2 );

        #endregion Generated Code
    }
}
