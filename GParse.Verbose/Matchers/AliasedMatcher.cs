using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class AliasedMatcher : BaseMatcher
    {
        internal readonly String Name;
        internal readonly BaseMatcher PatternMatcher;

        public AliasedMatcher ( BaseMatcher matcher, String Name )
        {
            this.Name = Name;
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return this.PatternMatcher.IsMatch ( reader, offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? this.PatternMatcher.Match ( reader ) : null;
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return this.PatternMatcher.InternalIsMatchExpression ( reader, offset );
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            return this.PatternMatcher.InternalMatchExpression ( reader );
        }
    }
}
