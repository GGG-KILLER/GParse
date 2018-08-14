using System;
using System.Diagnostics;
using GParse.Verbose.Parsing;
using GParse.Verbose.Matchers;
using GParse.Verbose.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Verbose.Tests
{
    [TestClass]
    public class MatchExpressionParserTests
    {
        private const Double TicksPerMicrosecond = ( TimeSpan.TicksPerMillisecond / 1000D );
        private readonly ExpressionParser expressionParser = new ExpressionParser ( );
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
                ( @"[a]", new AlternatedMatcher (
                    new CharMatcher ( 'a' )
                ) ),
                ( @"[\d]", new AlternatedMatcher (
                    new RangeMatcher ( '0', '9' )
                ) ),
                ( @"[\w]", new AlternatedMatcher (
                    new RangeMatcher ( 'A', 'Z' ),
                    new RangeMatcher ( 'a', 'z' ),
                    new RangeMatcher ( '0', '9' ),
                    new CharMatcher ( '_' )
                ) ),
                ( @"[a\w]", new AlternatedMatcher (
                    new CharMatcher ( 'a' ),
                    new RangeMatcher ( 'A', 'Z' ),
                    new RangeMatcher ( 'a', 'z' ),
                    new RangeMatcher ( '0', '9' ),
                    new CharMatcher ( '_' )
                ) ),
                ( @"'a' 'b'", new SequentialMatcher (
                    new CharMatcher ( 'a' ),
                    new CharMatcher ( 'b' )
                ) ),
                ( @"'a' | 'b'", new AlternatedMatcher (
                    new CharMatcher ( 'a' ),
                    new CharMatcher ( 'b' )
                ) ),
                ( @"\p{Alnum} \w*", new SequentialMatcher (
                    new AlternatedMatcher (
                        new RangeMatcher ( 'A', 'Z' ),
                        new RangeMatcher ( 'a', 'z' ),
                        new RangeMatcher ( '0', '9' )
                    ),
                    new AlternatedMatcher (
                        new RangeMatcher ( 'A', 'Z' ),
                        new RangeMatcher ( 'a', 'z' ),
                        new RangeMatcher ( '0', '9' ),
                        new CharMatcher ( '_' )
                    ).Infinite ( )
                ) ),
                ( @"ws ident ws i:'(' ws ( ident ( ws i:',' ws ident )* ws )? i:')' ws",
                new SequentialMatcher (
                    new RulePlaceholder ( "ws" ),
                    new RulePlaceholder ( "ident" ),
                    new RulePlaceholder ( "ws" ),
                    new IgnoreMatcher ( new CharMatcher ( '(' ) ),
                    new RulePlaceholder ( "ws" ),
                    new OptionalMatcher ( new SequentialMatcher (
                        new RulePlaceholder ( "ident" ),
                        new RepeatedMatcher ( new SequentialMatcher (
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

        [TestMethod]
        public void MemoizationTest ( )
        {
            BaseMatcher matcher = this.expressionParser.Parse ( "a a" );
            var typedm = ( matcher as SequentialMatcher );

            Assert.IsInstanceOfType ( matcher, typeof ( SequentialMatcher ) );
            Assert.AreSame ( typedm.PatternMatchers[0], typedm.PatternMatchers[1] );
        }
    }
}
