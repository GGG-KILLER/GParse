using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class RepeatedMatcher : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;
        internal readonly Int32 Limit;

        public RepeatedMatcher ( BaseMatcher matcher, Int32 limit )
        {
            this.PatternMatcher = matcher;
            this.Limit = limit;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return !reader.EOF ( ) && this.PatternMatcher.IsMatch ( reader, offset );
        }


        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return this.PatternMatcher.IsMatchExpression ( reader, offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader ) )
            {
                var sb = new StringBuilder ( );
                for ( var i = 0; i < this.Limit && this.IsMatch ( reader ); i++ )
                    sb.Append ( this.PatternMatcher.Match ( reader ) );
                return sb.ToString ( );
            }
            return null;
        }

        private static readonly MethodInfo SBAppend = typeof ( StringBuilder ).GetMethod ( "Append", new[] { typeof ( Object ) } );
        private static readonly MethodInfo SBToString = typeof ( StringBuilder ).GetMethod ( "ToString", Type.EmptyTypes );
        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            ParameterExpression sb = Expression.Variable ( typeof ( StringBuilder ), "sb" );
            ParameterExpression i = Expression.Variable ( typeof ( Int32 ), "i" );
            LabelTarget @return = Expression.Label ( typeof ( String ) );
            return Expression.Block (
                // Local vars
                new[] { sb, i },
                Expression.Assign ( sb, Expression.New ( typeof ( StringBuilder ) ) ),
                Expression.Assign ( i, Expression.Constant ( 0 ) ),
                Expression.Loop (
                    Expression.IfThenElse (
                        Expression.LessThan ( i, Expression.Constant ( this.Limit ) ),
                        Expression.Call ( sb, SBAppend, this.PatternMatcher.MatchExpression ( reader ) ),
                        Expression.Break ( @return, Expression.Call ( sb, SBToString ) )
                    ),
                    @return
                ),
                Expression.Label ( @return )
            );
        }
    }
}
