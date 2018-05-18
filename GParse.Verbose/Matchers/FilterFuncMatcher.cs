using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class FilterFuncMatcher : BaseMatcher
    {
        internal Func<Char, Boolean> Filter;
        internal String FullFilterName;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
            this.FullFilterName = $"{( Filter.Target != null ? Filter.Target.GetType ( ).FullName : Filter.Method.DeclaringType.FullName )}.{Filter.Method.Name}";
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var res = !reader.EOF ( ) && this.Filter ( ( Char ) reader.Peek ( offset ) );
            length = res ? 1 : 0;
            return res;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader, out var _ )
                ? new[] { reader.ReadString ( 1 ) }
                : throw new ParseException ( reader.Location, $"Character '{( Char ) reader.Peek ( )}' did not pass the validation filter ({this.FullFilterName})." );
        }
    }
}
