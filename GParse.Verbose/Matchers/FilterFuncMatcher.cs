using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class FilterFuncMatcher : BaseMatcher
    {
        internal Func<Char, Boolean> Filter;

        public FilterFuncMatcher ( Func<Char, Boolean> Filter )
        {
            this.Filter = Filter ?? throw new ArgumentNullException ( nameof ( Filter ) );
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && this.Filter ( ( Char ) reader.Peek ( ) );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
