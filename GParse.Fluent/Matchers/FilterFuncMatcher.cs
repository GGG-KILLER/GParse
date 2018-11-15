using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    /// <summary>
    /// Matches characters based on a filter function
    /// </summary>
    public sealed class FilterFuncMatcher : BaseMatcher, IEquatable<FilterFuncMatcher>
    {
        /// <summary>
        /// The filter it uses to match
        /// </summary>
        public readonly Func<Char, Boolean> Filter;

        /// <summary>
        /// The full name of the filter
        /// </summary>
        public readonly String FullFilterName;

        /// <summary>
        /// Initializes this
        /// </summary>
        /// <param name="Filter"></param>
        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
            this.FullFilterName = $"{( Filter.Target != null ? Filter.Target.GetType ( ).FullName : Filter.Method.DeclaringType.FullName )}.{Filter.Method.Name}";
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
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as FilterFuncMatcher );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( FilterFuncMatcher other ) => other != null &&
                    EqualityComparer<Func<Char, Boolean>>.Default.Equals ( this.Filter, other.Filter ) &&
                     this.FullFilterName == other.FullFilterName;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 1232269394;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Char, Boolean>>.Default.GetHashCode ( this.Filter );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.FullFilterName );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator == ( FilterFuncMatcher matcher1, FilterFuncMatcher matcher2 ) => EqualityComparer<FilterFuncMatcher>.Default.Equals ( matcher1, matcher2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="matcher1"></param>
        /// <param name="matcher2"></param>
        /// <returns></returns>
        public static Boolean operator != ( FilterFuncMatcher matcher1, FilterFuncMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
