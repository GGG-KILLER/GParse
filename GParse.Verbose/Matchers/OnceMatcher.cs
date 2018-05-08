using System;
using System.Linq.Expressions;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class OnceMatcher : BaseMatcher
    {
        internal IPatternMatcher PatternMatcher;
        private Boolean Matched;

        public OnceMatcher ( IPatternMatcher patternMatcher )
        {
            this.PatternMatcher = patternMatcher;
            this.Matched = false;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return this.Matched && this.PatternMatcher.IsMatch ( reader, offset );
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            throw new InvalidOperationException ( "Cannot transform stateful matchers into expressions." );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.Matched )
            {
                this.Matched = true;
                return this.PatternMatcher.Match ( reader );
            }
            return null;
        }

        public override void ResetInternalState ( )
        {
            this.Matched = false;
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            throw new InvalidOperationException ( "Cannot transform stateful matchers into expressions." );
        }
    }
}
