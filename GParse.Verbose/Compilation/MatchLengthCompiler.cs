using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Compilation
{
    public delegate Int32 MatcherMatchLength ( SourceCodeReader reader, Int32 offset = 0 );
    public class MatchLengthCompiler : MatcherTreeVisitor<Expression>
    {
        private readonly Dictionary<String, MatcherMatchLength> Cache = new Dictionary<String, MatcherMatchLength> ( );
        private readonly ParameterExpression Reader;

        public MatchLengthCompiler ( ParameterExpression reader )
        {
            this.Reader = reader;
        }

        public MatcherMatchLength GetDelegate ( BaseMatcher matcher )
        {
            return Expression.Lambda<MatcherMatchLength> ( this.Visit ( matcher ), this.Reader ).Compile ( );
        }

        public override Expression Visit ( AllMatcher allMatcher )
        {
            var body = new List<Expression> ( );
            ParameterExpression len = Expression.Variable ( typeof ( Int32 ), "len" );
            LabelTarget ret = Expression.Label ( typeof ( Int32 ) );

            for ( var i = 0; i < allMatcher.PatternMatchers.Length; i++ )
                body.Add (
                    Expression.IfThen (
                        Expression.Equal ( Expression.AddAssign ( len, this.Visit ( allMatcher.PatternMatchers[i] ) ), Expression.Constant ( -1 ) ),
                        Expression.Return ( ret, Expression.Constant ( -1 ), typeof ( Int32 ) ) ) );
            body.Add ( Expression.Return ( ret, len, typeof ( Int32 ) ) );

            // Self calling function (can probably be optimized
            // but I have no idea how)
            return ExpressionUtils.GetLambdaExpressionCallExpression (
                Expression.Lambda<Func<SourceCodeReader, Int32>> ( Expression.Block ( body.ToArray ( ) ), this.Reader ) );
        }

        public override Expression Visit ( AnyMatcher anyMatcher )
        {
            var body = new List<Expression> ( );
            ParameterExpression len = Expression.Variable ( typeof ( Int32 ), "len" );
            LabelTarget ret = Expression.Label ( typeof ( Int32 ) );

            for ( var i = 0; i < anyMatcher.PatternMatchers.Length; i++ )
                body.Add (
                    Expression.IfThen (
                        Expression.NotEqual ( Expression.Assign ( len, this.Visit ( anyMatcher.PatternMatchers[i] ) ), Expression.Constant ( -1 ) ),
                        Expression.Return ( ret, len, typeof ( Int32 ) ) ) );
            body.Add ( Expression.Return ( ret, Expression.Constant ( -1 ), typeof ( Int32 ) ) );

            // Self calling function (can probably be optimized
            // but I have no idea how)
            return ExpressionUtils.GetLambdaExpressionCallExpression (
                Expression.Lambda<Func<SourceCodeReader, Int32>> ( Expression.Block ( body.ToArray ( ) ), this.Reader ) );
        }

        public override Expression Visit ( CharMatcher charMatcher )
        {
            return Expression.Condition ( Expression.Equal (
                ExpressionUtils.GetMethodCallExpression<SourceCodeReader, Int32> ( this.Reader, "Peek", Expression.Constant ( 0 ) ),
                Expression.Constant ( charMatcher.Filter ) ),
                Expression.Constant ( 1 ),
                Expression.Constant ( -1 ) );
        }

        public override Expression Visit ( CharRangeMatcher charRangeMatcher )
        {
            Expression peek = ExpressionUtils.GetMethodCallExpression<SourceCodeReader, Int32> ( this.Reader, "Peek", Expression.Constant ( 0 ) );
            return Expression.Condition (
                Expression.And (
                    Expression.LessThan ( peek, Expression.Constant ( charRangeMatcher.Start ) ),
                    Expression.GreaterThan ( peek, Expression.Constant ( charRangeMatcher.End ) ) ),
                Expression.Constant ( 1 ),
                Expression.Constant ( -1 ) );
        }

        public override Expression Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            Func<Char, Boolean> del = filterFuncMatcher.Filter;
            return Expression.Call (
                del.Target != null ? Expression.Constant ( del.Target ) : null,
                del.Method,
                ExpressionUtils.GetMethodCallExpression<SourceCodeReader, Int32> ( this.Reader, "Peek", Expression.Constant ( 0 ) ) );
        }

        public override Expression Visit ( MultiCharMatcher multiCharMatcher )
        {
            Expression peek = ExpressionUtils.GetMethodCallExpression<SourceCodeReader, Int32> ( this.Reader, "Peek", Expression.Constant ( 0 ) );
            BinaryExpression cond = Expression.Equal ( peek, Expression.Constant ( multiCharMatcher.Whitelist[0] ) );
            for ( var i = 1; i < multiCharMatcher.Whitelist.Length; i++ )
                cond = Expression.Or ( cond, Expression.Equal ( peek, Expression.Constant ( multiCharMatcher.Whitelist[i] ) ) );
            return Expression.Condition ( cond, Expression.Constant ( 1 ), Expression.Constant ( -1 ) );
        }

        public override Expression Visit ( RulePlaceholder rulePlaceholder )
        {
            return Expression.Call ( Expression.Constant ( rulePlaceholder ),
                ( ( MatcherMatchLength ) rulePlaceholder.MatchLength ).Method );
        }

        public override Expression Visit ( StringMatcher stringMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( IgnoreMatcher ignoreMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( JoinMatcher joinMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( NegatedMatcher negatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( OptionalMatcher optionalMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( RepeatedMatcher repeatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( RuleWrapper ruleWrapper )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( MarkerMatcher markerMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( EOFMatcher eofMatcher )
        {
            throw new NotImplementedException ( );
        }
    }
}
