using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class InfiniteMatcher : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;

        public InfiniteMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset )
        {
            return !reader.EOF ( ) && this.PatternMatcher.IsMatch ( reader, offset );
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return this.PatternMatcher.InternalIsMatchExpression ( reader, offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader, 0 ) )
            {
                var sb = new StringBuilder ( );
                while ( this.IsMatch ( reader, 0 ) )
                    sb.Append ( this.PatternMatcher.Match ( reader ) );
                return sb.ToString ( );
            }
            return null;
        }

        private static readonly MethodInfo SBAppend = typeof ( StringBuilder ).GetMethod ( "Append", new[] { typeof ( Object ) } );
        private static readonly MethodInfo SBToString = typeof ( StringBuilder ).GetMethod ( "ToString", Type.EmptyTypes );
        internal override Expression InternalMatchExpression ( ParameterExpression reader, ParameterExpression MatchedListener )
        {
            ParameterExpression sb = Expression.Variable ( typeof ( StringBuilder ), "sb" );
            LabelTarget @return = Expression.Label ( typeof ( String ) );
            return Expression.Block (
                // Local vars
                new[] { sb },
                Expression.Assign ( sb, Expression.New ( typeof ( StringBuilder ) ) ),
                Expression.Loop (
                    Expression.IfThenElse (
                        this.IsMatchExpression ( reader, Expression.Constant ( 0 ) ),
                        Expression.Call ( sb, SBAppend, this.PatternMatcher.MatchExpression ( reader, MatchedListener ) ),
                        Expression.Break ( @return, Expression.Call ( sb, SBToString ) )
                    ),
                    @return
                ),
                Expression.Label ( @return )
            );
        }
    }
}
