using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class AllMatcher : BaseMatcher
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AllMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            foreach ( BaseMatcher pm in this.PatternMatchers )
                if ( !pm.IsMatch ( reader, offset++ ) )
                    return false;
            return true;
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            Expression root = this.PatternMatchers[0].InternalIsMatchExpression ( reader, offset );
            for ( var i = 1; i < this.PatternMatchers.Length; i++ )
                root = Expression.And (
                    root,
                    this.PatternMatchers[i].InternalIsMatchExpression (
                        reader,
                        Expression.Add ( offset, Expression.Constant ( i ) ) ) );
            return root;
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;

            var sb = new StringBuilder ( );
            foreach ( IPatternMatcher pm in this.PatternMatchers )
                sb.Append ( pm.Match ( reader ) );
            return sb.ToString ( );
        }

        private static readonly MethodInfo SBAppend = typeof ( StringBuilder ).GetMethod ( "Append", new[] { typeof ( Object ) } );
        private static readonly MethodInfo SBToString = typeof ( StringBuilder ).GetMethod ( "ToString", Type.EmptyTypes );

        internal override Expression InternalMatchExpression ( ParameterExpression reader, ParameterExpression MatchedListener )
        {
            // Build expression body
            ParameterExpression sb = Expression.Variable ( typeof ( StringBuilder ), "sb" );
            var i = 0;
            var body = new Expression[this.PatternMatchers.Length + 3];
            body[i++] = Expression.Assign ( sb, Expression.New ( typeof ( StringBuilder ) ) );
            for ( ; i < this.PatternMatchers.Length; i++ )
                body[i] = Expression.Call ( sb, SBAppend, this.PatternMatchers[i].InternalMatchExpression ( reader, MatchedListener ) );
            // Return
            LabelTarget @return = Expression.Label ( typeof ( String ) );
            body[i++] = Expression.Return ( @return, Expression.Call ( sb, SBToString ) );
            body[i] = Expression.Label ( @return );
            // Return body with local variable
            return Expression.Block ( new[] { sb }, body );
        }

        public override void ResetInternalState ( )
        {
            foreach ( IPatternMatcher pm in this.PatternMatchers )
                pm.ResetInternalState ( );
        }
    }
}
