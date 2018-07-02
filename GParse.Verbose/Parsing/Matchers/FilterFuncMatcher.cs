using System;
using System.Collections.Generic;
using GParse.Verbose.Parsing.Abstractions;

namespace GParse.Verbose.Parsing.Matchers
{
    public sealed class FilterFuncMatcher : BaseMatcher, IEquatable<FilterFuncMatcher>
    {
        public readonly Func<Char, Boolean> Filter;
        public readonly String FullFilterName;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
            this.FullFilterName = $"{( Filter.Target != null ? Filter.Target.GetType ( ).FullName : Filter.Method.DeclaringType.FullName )}.{Filter.Method.Name}";
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as FilterFuncMatcher );
        }

        public Boolean Equals ( FilterFuncMatcher other )
        {
            return other != null &&
                    EqualityComparer<Func<Char, Boolean>>.Default.Equals ( this.Filter, other.Filter ) &&
                     this.FullFilterName == other.FullFilterName;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 1232269394;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Char, Boolean>>.Default.GetHashCode ( this.Filter );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.FullFilterName );
            return hashCode;
        }

        public static Boolean operator == ( FilterFuncMatcher matcher1, FilterFuncMatcher matcher2 ) => EqualityComparer<FilterFuncMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( FilterFuncMatcher matcher1, FilterFuncMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
