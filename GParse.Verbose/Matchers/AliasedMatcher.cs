using System;
using System.Linq.Expressions;
using System.Reflection;
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
            if ( !this.IsMatch ( reader ) )
                return null;

            var match = this.PatternMatcher.Match ( reader );
            try
            {
                return match;
            }
            finally
            {
                this.OnMatch ( this.Name, match );
            }
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return this.PatternMatcher.InternalIsMatchExpression ( reader, offset );
        }

        private static readonly MethodInfo ActionStringStringInvoke = typeof ( Action<String, String> )
            .GetMethod ( "Invoke", new[] { typeof ( String ), typeof ( String ) } );
        internal override Expression InternalMatchExpression ( ParameterExpression reader, ParameterExpression MatchedListener )
        {
            ParameterExpression match = Expression.Variable ( typeof ( String ), "match" );
            LabelTarget @return = Expression.Label ( typeof ( String ) );
            return Expression.Block (
                new[] { match },
                Expression.Assign ( match, this.PatternMatcher.InternalMatchExpression ( reader, MatchedListener ) ),
                Expression.TryFinally (
                    Expression.Return ( @return, match ),
                    Expression.Call ( MatchedListener, ActionStringStringInvoke, Expression.Constant ( this.Name ), match )
                ),
                Expression.Label ( @return )
            );
        }
    }
}
