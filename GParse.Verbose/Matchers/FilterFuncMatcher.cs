using System;
using System.Collections.Generic;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class FilterFuncMatcher : BaseMatcher, IEquatable<FilterFuncMatcher>
    {
        internal readonly Func<Char, Boolean> Filter;
        internal readonly String FullFilterName;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
            this.FullFilterName = $"{( Filter.Target != null ? Filter.Target.GetType ( ).FullName : Filter.Method.DeclaringType.FullName )}.{Filter.Method.Name}";
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return !reader.EOF ( ) && this.Filter ( ( Char ) reader.Peek ( offset ) ) ? 1 : -1;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.MatchLength ( reader ) != -1
                ? new[] { reader.ReadString ( 1 ) }
                : throw new ParseException ( reader.Location, $"Character '{( Char ) reader.Peek ( )}' did not pass the validation filter ({this.FullFilterName})." );
        }

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
