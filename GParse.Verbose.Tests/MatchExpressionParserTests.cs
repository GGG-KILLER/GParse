using System;
using System.Diagnostics;
using GParse.Verbose.Matchers;
using GParse.Verbose.Parsing;
using GParse.Verbose.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Verbose.Tests
{
    [TestClass]
    public class MatchExpressionParserTests
    {
        private const Double TicksPerMicrosecond = ( TimeSpan.TicksPerMillisecond / 1000D );
        private readonly MatchExpressionParser expressionParser = new MatchExpressionParser ( );
        private readonly ExpressionReconstructor exprReconstructor = new ExpressionReconstructor ( );

        public void TestExpr ( String expression, BaseMatcher expected )
        {
            Logger.LogMessage ( "Testing {0} against {1}", expression,
                expected.Accept ( this.exprReconstructor ) );
            var sw = Stopwatch.StartNew ( );
            BaseMatcher result = this.expressionParser.Parse ( expression );
            sw.Stop ( );
            Logger.LogMessage ( "[{1:00.0##}μs] Actual result: {0}", result.Accept ( this.exprReconstructor ), sw.ElapsedTicks / TicksPerMicrosecond );
            Assert.AreEqual ( expected, result );
        }

        [TestMethod]
        public void ParseTest ( )
        {
            var testMatrix = new (String, BaseMatcher)[]
            {
                ( @"'hi'", new StringMatcher ( "hi" ) ),
                ( @"[a]", new AnyMatcher (
                    new CharMatcher ( 'a' )
                ) ),
                ( @"[\d]", new AnyMatcher (
                    new CharRangeMatcher ( '0', '9' )
                ) ),
                ( @"[\w]", new AnyMatcher (
                    new CharRangeMatcher ( 'A', 'Z' ),
                    new CharRangeMatcher ( 'a', 'z' ),
                    new CharRangeMatcher ( '0', '9' ),
                    new CharMatcher ( '_' )
                ) ),
                ( @"[a\w]", new AnyMatcher (
                    new CharMatcher ( 'a' ),
                    new CharRangeMatcher ( 'A', 'Z' ),
                    new CharRangeMatcher ( 'a', 'z' ),
                    new CharRangeMatcher ( '0', '9' ),
                    new CharMatcher ( '_' )
                ) ),
                ( @"'a' 'b'", new AllMatcher (
                    new CharMatcher ( 'a' ),
                    new CharMatcher ( 'b' )
                ) ),
                ( @"'a' | 'b'", new AnyMatcher (
                    new CharMatcher ( 'a' ),
                    new CharMatcher ( 'b' )
                ) ),
                ( @"\p{Alnum} \w*", new AllMatcher (
                    new AnyMatcher (
                        new CharRangeMatcher ( 'A', 'Z' ),
                        new CharRangeMatcher ( 'a', 'z' ),
                        new CharRangeMatcher ( '0', '9' )
                    ),
                    new AnyMatcher (
                        new CharRangeMatcher ( 'A', 'Z' ),
                        new CharRangeMatcher ( 'a', 'z' ),
                        new CharRangeMatcher ( '0', '9' ),
                        new CharMatcher ( '_' )
                    ).Infinite ( )
                ) ),
                ( @"ws ident ws i:'(' ws ( ident ( ws i:',' ws ident )* ws )? i:')' ws",
                new AllMatcher (
                    new RulePlaceholder ( "ws" ),
                    new RulePlaceholder ( "ident" ),
                    new RulePlaceholder ( "ws" ),
                    new IgnoreMatcher ( new CharMatcher ( '(' ) ),
                    new RulePlaceholder ( "ws" ),
                    new OptionalMatcher ( new AllMatcher (
                        new RulePlaceholder ( "ident" ),
                        new RepeatedMatcher ( new AllMatcher (
                            new RulePlaceholder ( "ws" ),
                            new IgnoreMatcher ( new CharMatcher ( ',' ) ),
                            new RulePlaceholder ( "ws" ),
                            new RulePlaceholder ( "ident" )
                        ) ),
                        new RulePlaceholder ( "ws" )
                    ) ),
                    new IgnoreMatcher ( new CharMatcher ( ')' ) ),
                    new RulePlaceholder ( "ws" )
                ) )
            };

            foreach ( (String Expr, BaseMatcher Expected) in testMatrix )
                this.TestExpr ( Expr, Expected );
        }
    }
}
