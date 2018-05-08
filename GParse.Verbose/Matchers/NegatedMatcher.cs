using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class NegatedMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;

        public NegatedMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return !this.PatternMatcher.IsMatch ( reader, offset );
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.Negate ( this.PatternMatcher.InternalIsMatchExpression ( reader, offset ) );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? "" : null;
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader, ParameterExpression MatchedListener )
        {
            return Expression.Constant ( String.Empty );
        }
    }
}
