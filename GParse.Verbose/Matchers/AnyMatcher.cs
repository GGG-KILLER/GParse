using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class AnyMatcher : BaseMatcher
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AnyMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader, offset ) )
                    return true;
            return false;
        }

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            Expression root = this.PatternMatchers[0].InternalIsMatchExpression ( reader, offset );
            for ( var i = 1; i < this.PatternMatchers.Length; i++ )
                root = Expression.Or ( root, this.PatternMatchers[i].InternalIsMatchExpression ( reader, offset ) );
            return root;
        }

        public override String Match ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader ) )
                    return matcher.Match ( reader );
            return null;
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            var body = new Expression[this.PatternMatchers.Length + 1];
            var i = 0;
            LabelTarget @return = Expression.Label ( typeof ( String ) );

            for ( ; i < this.PatternMatchers.Length; i++ )
                body[i] = Expression.IfThen (
                    // If is match
                    this.PatternMatchers[i].IsMatchExpression ( reader, Expression.Constant ( 0 ) ),
                    // return match
                    Expression.Return ( @return, this.PatternMatchers[i].InternalMatchExpression ( reader ) )
                );
            // return label
            body[i] = Expression.Label ( @return );

            return Expression.Block ( body );
        }

        public override void ResetInternalState ( )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                matcher.ResetInternalState ( );
        }
    }
}
