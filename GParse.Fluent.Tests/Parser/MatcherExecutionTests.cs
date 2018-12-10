using System;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Fluent.Tests.Parser
{
    [TestClass]
    public class MatcherExecutionTests
    {
        [TestMethod]
        public void AlternatedMatcherTest ( ) =>
            Test ( new AlternatedMatcher ( new StringMatcher ( "aa" ), new StringMatcher ( "bb" ) ),
                new[]
                {
                    new[] { "aa" }, new[] { "bb" }, new[] { "aa" }, new[] { "bb" }
                }, new[] { "aa", "bb", "aabb", "bbaa" }, new[] { "a", "b", "cc", "dd" } );

        [TestMethod]
        public void CharListMatcherTest ( ) =>
            Test ( new CharListMatcher ( 'a', 'b', 'c' ), new[]
            {
                new[] { "a" }, new[] { "b" }, new[] { "c" }, new[] { "a" }, new[] { "b" },
                new[] { "c" },
            }, new[] { "a", "b", "c", "aa", "bb", "cc" }, new[] { "d", "e", "f", "g", "h", "i" } );

        [TestMethod]
        public void CharMatcherTest ( ) =>
            Test ( new CharMatcher ( 'a' ), new[]
            {
                new[] { "a" }, new[] { "a" }, new[] { "a" }, new[] { "a" },
            }, new[] { "a", "aa", "aaa", "aaaa" }, new[] { "b", "c", "d", "e", "f", " a" } );

        [TestMethod]
        public void EOFMatcherTest ( ) =>
            Test ( new EOFMatcher ( ), Array.Empty<String[]> ( ), new[] { "" },
                new[] { "a", "b", "c" } );

        [TestMethod]
        public void FilterFuncMatcherTest ( ) =>
            Test ( new FilterFuncMatcher ( Char.IsDigit ), new[]
            {
                new[] { "0" }, new[] { "1" }, new[] { "2" }, new[] { "3" }, new[] { "4" },
                new[] { "5" }, new[] { "6" }, new[] { "7" }, new[] { "8" },
            }, new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" }, new[] { "a", "b", "c" } );

        [TestMethod]
        public void SavingAndLoadingMatchersTest ( ) =>
            Test ( new SequentialMatcher (  // Hack for matching any char ↓
                new SavingMatcher ( "test", new CharListMatcher (
                    'a', 'b', 'c', 'd', 'e', 'f'
                ) ),
                new LoadingMatcher ( "test" )
            ), new[]
            {
                new[] { "a", "a" }, new[] { "b", "b" }, new[] { "c", "c" },
                new[] { "e", "e" }, new[] { "f", "f" }
            }, new[] { "aa", "bb", "cc", "ee", "ff" }, new[] { "ab", "bc", "cd", "ef", "ba" } );

        [TestMethod]
        public void RangeMatcher ( ) =>
            Test ( new RangeMatcher ( '0', '9' ), new[]
            {
                new[] { "0" }, new[] { "2" }, new[] { "9" }
            }, new[] { "0", "2", "9" }, new[] { "a", "b", "c" } );

        [TestMethod]
        public void SequentialMatcherTest ( ) =>
            Test ( new SequentialMatcher ( new CharMatcher ( 'a' ), new CharMatcher ( 'b' ) ), new[]
                {
                    new[] { "a", "b" }
                }, new[] { "ab" }, new[] { "ba" } );

        [TestMethod]
        public void StringMatcherTest ( ) =>
            Test ( new StringMatcher ( "aa" ), new[]
                        {
                new[] { "aa" }, new[] { "aa" }, new[] { "aa" }, new[] { "aa" },
            }, new[] { "aa", "aaa", "aaaa" }, new[] { "a", " a", " aa", "bb", "ba", "ca" } );

        [TestMethod]
        public void IgnoreMatcherTest ( ) =>
            Test ( new IgnoreMatcher ( new CharMatcher ( 'a' ) ), new[]
            {
                Array.Empty<String> ( )
            }, new[] { "a" }, Array.Empty<String> ( ) );// There are no failure cases for this ↑

        [TestMethod]
        public void JoinMatcher ( ) =>
            Test ( new JoinMatcher (
                new SequentialMatcher ( new CharMatcher ( 'a' ), new CharMatcher ( 'b' ) ) ), new[]
                {
                    new[] { "ab" }
                }, new[] { "ab" }, Array.Empty<String> ( ) );// No real failure cases for this either ↑

        [TestMethod]
        public void NegatedMatcher ( ) =>
            Test ( new NegatedMatcher ( new CharMatcher ( 'a' ) ), new[]
            {
                new[] { "b" }, new[] { "d" }, new[] { "c" }
            }, new[] { "b", "d", "c" }, new[] { "a" } );

        private static void Test ( BaseMatcher matcher, String[][] strings, String[] success, String[] error )
        {
            var idx = -1;
            var called = false;
            var parser = new MatcherExecutionTestParser<Object> ( matcher, ( _, result ) =>
            {
                Assert.IsTrue ( result.Success );

                if ( strings.Length > idx )
                    CollectionAssert.AreEqual ( strings[idx], result.Strings, $"[{ String.Join ( ", ", strings[idx] ) }] ≠ [{String.Join ( ", ", result.Strings )}]" );

                called = true;
                return null;
            }, ( _, __ ) => null );

            var rec = new ExpressionReconstructor ( );
            if ( !( matcher is FilterFuncMatcher ) )
                Logger.LogMessage ( $"Initiating testing of: {matcher.Accept ( rec )}" );
            Logger.LogMessage ( $"{success.Length} successful test cases, and {error.Length} unsucessful test cases expected." );

            foreach ( var str in success )
            {
                idx++;
                called = false;

                Logger.LogMessage ( $@"Running iteration #{idx}.
Test data:" );
                if ( strings.Length > idx )
                    Logger.LogMessage ( $"Strings: [{String.Join ( ", ", strings[idx] )}]" );
                Logger.LogMessage ( $"Input:   {str}" );
                Logger.LogMessage ( "Expected result: success." );
                parser.Parse ( str );
                Assert.IsTrue ( called );
            }

            foreach ( var str in error )
            {
                called = false;
                Assert.ThrowsException<MatcherFailureException> ( ( ) => parser.Parse ( str ) );
                Assert.IsFalse ( called );
            }
        }
    }
}
