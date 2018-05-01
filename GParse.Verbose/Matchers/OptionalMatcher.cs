using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class OptionalMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;

        public OptionalMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 ) => true;

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.Constant ( true );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.PatternMatcher.IsMatch ( reader ) ? this.PatternMatcher.Match ( reader ) : "";
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            return Expression.Condition (
                this.PatternMatcher.IsMatchExpression ( reader, Expression.Constant ( 0 ) ),
                this.PatternMatcher.InternalMatchExpression ( reader ),
                Expression.Constant ( "" )
            );
        }
    }
}
