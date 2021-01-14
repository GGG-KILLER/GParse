using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Lexing.Composable;
using GParse.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GParse.Lexing.Composable.NodeFactory;

namespace GParse.Tests.Lexing.Composable
{
    [TestClass]
    public class RegexParserTests
    {
        private static void AssertParse ( String pattern, GrammarNode<Char> expected )
        {
            // Act
            GrammarNode<Char> actual = RegexParser.Parse ( pattern );

            // Check
            var expectedString = GrammarNodeToStringConverter.Convert ( expected );
            var actualString = GrammarNodeToStringConverter.Convert ( actual );

            Assert.IsTrue ( GrammarTreeStructuralComparer.Instance.Equals ( expected, actual ), $"Expected /{expectedString}/ for /{pattern}/ but got /{actualString}/ instead." );
        }

        private static void AssertParseThrows ( String pattern, Int32 positionStart, Int32 positionEnd, String message )
        {
            RegexParseException exception = Assert.ThrowsException<RegexParseException> ( ( ) => RegexParser.Parse ( pattern ) );

            Assert.AreEqual ( new Range<Int32> ( positionStart, positionEnd ), exception.Range );
            Assert.AreEqual ( message, exception.Message );
        }

        [TestMethod]
        public void Parse_ParsesCharacterProperly ( )
        {
            AssertParse ( /*lang=regex*/"a", Terminal ( 'a' ) );
            AssertParse ( /*lang=regex*/" ", Terminal ( ' ' ) );
            AssertParse ( /*lang=regex*/"]", Terminal ( ']' ) );
            AssertParse ( /*lang=regex*/"}", Terminal ( '}' ) );
        }

        [TestMethod]
        public void Parse_ParsesEscapedCharactersProperly ( )
        {
            AssertParse ( /*lang=regex*/@"\a", Terminal ( '\a' ) );
            //AssertParse ( /*lang=regex*/@"\b", Terminal ( '\b' ) );
            AssertParse ( /*lang=regex*/@"\f", Terminal ( '\f' ) );
            AssertParse ( /*lang=regex*/@"\n", Terminal ( '\n' ) );
            AssertParse ( /*lang=regex*/@"\r", Terminal ( '\r' ) );
            AssertParse ( /*lang=regex*/@"\t", Terminal ( '\t' ) );
            AssertParse ( /*lang=regex*/@"\v", Terminal ( '\v' ) );
            AssertParse ( /*lang=regex*/@"\.", Terminal ( '.' ) );
            AssertParse ( /*lang=regex*/@"\$", Terminal ( '$' ) );
            AssertParse ( /*lang=regex*/@"\^", Terminal ( '^' ) );
            AssertParse ( /*lang=regex*/@"\{", Terminal ( '{' ) );
            AssertParse ( /*lang=regex*/@"\[", Terminal ( '[' ) );
            AssertParse ( /*lang=regex*/@"\(", Terminal ( '(' ) );
            AssertParse ( /*lang=regex*/@"\|", Terminal ( '|' ) );
            AssertParse ( /*lang=regex*/@"\)", Terminal ( ')' ) );
            AssertParse ( /*lang=regex*/@"\*", Terminal ( '*' ) );
            AssertParse ( /*lang=regex*/@"\+", Terminal ( '+' ) );
            AssertParse ( /*lang=regex*/@"\?", Terminal ( '?' ) );
            AssertParse ( /*lang=regex*/@"\\", Terminal ( '\\' ) );

            AssertParse ( /*lang=regex*/@"\x0A", Terminal ( '\x0A' ) );

            AssertParseThrows ( @"\b", 0, 2, "Invalid escape sequence." );
            AssertParseThrows ( @"\g", 0, 2, "Invalid escape sequence." );
        }

        [TestMethod]
        public void Parse_ParsesCharacterClasses ( )
        {
            AssertParse ( /*lang=regex*/@".", CharacterClasses.Dot );
            AssertParse ( /*lang=regex*/@"\d", CharacterClasses.Digit );
            AssertParse ( /*lang=regex*/@"\D", CharacterClasses.NotDigit );
            AssertParse ( /*lang=regex*/@"\w", CharacterClasses.Word );
            AssertParse ( /*lang=regex*/@"\W", CharacterClasses.NotWord );
            AssertParse ( /*lang=regex*/@"\s", CharacterClasses.Whitespace );
            AssertParse ( /*lang=regex*/@"\S", CharacterClasses.NotWhitespace );
            foreach ( KeyValuePair<String, GrammarNode<Char>> pair in CharacterClasses.Unicode.AllCategories )
            {
                AssertParse ( /*lang=regex*/$@"\p{{{pair.Key}}}", pair.Value );
                AssertParse ( /*lang=regex*/$@"\P{{{pair.Key}}}", pair.Value.Negate ( ) );
            }

            AssertParseThrows (
                @"\p{Unexistent}",
                0, 14,
                "Invalid unicode class or code block name: Unexistent." );
        }

        [TestMethod]
        public void Parse_ParsesSets ( )
        {
            AssertParse ( /*lang=regex*/@"[abc]", Set ( 'a', 'b', 'c' ) );
            AssertParse ( /*lang=regex*/@"[.]", Set ( '.' ) );
            AssertParse ( /*lang=regex*/@"[a-z]", Set ( new Range<Char> ( 'a', 'z' ) ) );
            AssertParse ( /*lang=regex*/@"[\d\s]", Set ( CharacterClasses.Digit, CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[^\d\s]", !Set ( CharacterClasses.Digit, CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[^\D\S]", !Set ( CharacterClasses.NotDigit, CharacterClasses.NotWhitespace ) );
            AssertParse ( /*lang=regex*/@"[\d-\s]", Set ( CharacterClasses.Digit, '-', CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[]]", Set ( ']' ) );
            AssertParse ( /*lang=regex*/@"[^]]", !Set ( ']' ) );
            AssertParseThrows ( @"[]", 0, 2, "Unfinished set." );
            AssertParseThrows ( @"[^]", 0, 3, "Unfinished set." );
        }

        [TestMethod]
        public void Parse_ParsesPositiveLookahead ( )
        {
            AssertParse ( /*lang=regex*/@"(?=)", PositiveLookahead ( Sequence ( ) ) );
            AssertParse ( /*lang=regex*/@"(?=a)", PositiveLookahead ( Terminal ( 'a' ) ) );
            AssertParse ( /*lang=regex*/@"(?=[\d])", PositiveLookahead ( Set ( CharacterClasses.Digit ) ) );
            AssertParse ( /*lang=regex*/@"(?!)", !PositiveLookahead ( Sequence ( ) ) );
            AssertParse ( /*lang=regex*/@"(?!a)", !PositiveLookahead ( Terminal ( 'a' ) ) );
            AssertParse ( /*lang=regex*/@"(?![\d])", !PositiveLookahead ( Set ( CharacterClasses.Digit ) ) );

            AssertParseThrows ( @"(?", 0, 2, "Unrecognized group type." );
            AssertParseThrows ( @"(?=", 0, 3, "Unfinished lookahead." );
            AssertParseThrows ( @"(?!", 0, 3, "Unfinished lookahead." );
        }

        [TestMethod]
        public void Parse_ParsesNonCapturingGroup ( )
        {
            AssertParse ( /*lang=regex*/@"(?:)", Sequence ( ) );
            AssertParse ( /*lang=regex*/@"(?:a)", Terminal ( 'a' ) );
            AssertParse ( /*lang=regex*/@"(?:(?:[\d]))", Set ( CharacterClasses.Digit ) );

            AssertParseThrows ( @"(?", 0, 2, "Unrecognized group type." );
            AssertParseThrows ( @"(?:", 0, 3, "Unfinished non-capturing group." );
        }

        [TestMethod]
        [SuppressMessage ( "Style", "RE0001:Regex issue: {0}", Justification = "Backreferences won't be valid since they're being parsed on their own." )]
        public void Parse_ParsesNumberedBackreference ( )
        {
            AssertParse ( /*lang=regex*/@"\1", Backreference ( 1 ) );
            AssertParse ( /*lang=regex*/@"\10", Backreference ( 10 ) );
            AssertParse ( /*lang=regex*/@"\100", Backreference ( 100 ) );

            AssertParseThrows (
                @"\1000",
                0, 5,
                "Invalid backreference." );
        }

        [TestMethod]
        [SuppressMessage ( "Style", "RE0001:Regex issue: {0}", Justification = "Backreferences won't be valid since they're being parsed on their own." )]
        public void Parse_ParsesNamedBackreference ( )
        {
            AssertParse ( /*lang=regex*/@"\k<a>", Backreference ( "a" ) );
            AssertParse ( /*lang=regex*/@"\k<something>", Backreference ( "something" ) );

            AssertParseThrows ( @"\k", 0, 2, "Expected opening '<' for named backreference." );
            AssertParseThrows ( @"\k<", 0, 3, "Invalid named backreference name." );
            AssertParseThrows ( @"\k<#", 0, 3, "Invalid named backreference name." );
            AssertParseThrows ( @"\k<>", 0, 3, "Invalid named backreference name." );
            AssertParseThrows ( @"\k<a", 0, 4, "Expected closing '>' in named backreference." );
        }

        [TestMethod]
        public void Parse_ParsesNumberedCaptureGroup ( )
        {
            AssertParse ( /*lang=regex*/@"()", Capture ( 1, Sequence ( ) ) );
            AssertParse ( /*lang=regex*/@"(a)", Capture ( 1, Terminal ( 'a' ) ) );

            AssertParseThrows ( @"(", 0, 1, "Expected closing ')' for capture group." );
            AssertParseThrows ( @"(a", 0, 2, "Expected closing ')' for capture group." );
        }

        [TestMethod]
        public void Parse_ParsesNamedCaptureGroup ( )
        {
            AssertParse ( /*lang=regex*/@"(?<test>)", Capture ( "test", Sequence ( ) ) );
            AssertParse ( /*lang=regex*/@"(?<test>a)", Capture ( "test", Terminal ( 'a' ) ) );

            AssertParseThrows ( @"(?", 0, 2, "Unrecognized group type." );
            AssertParseThrows ( @"(?<", 0, 3, "Invalid named capture group name." );
            AssertParseThrows ( @"(?<#", 0, 3, "Invalid named capture group name." );
            AssertParseThrows ( @"(?<a", 0, 4, "Expected closing '>' for named capture group name." );
            AssertParseThrows ( @"(?<a>", 0, 5, "Expected closing ')' for named capture group." );
        }

        [TestMethod]
        public void Parse_ParsesRepetitions ( )
        {
            AssertParse ( /*lang=regex*/"a?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 0, 1 ) ) );
            AssertParse ( /*lang=regex*/"a*", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 0, null ) ) );
            AssertParse ( /*lang=regex*/"a+", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 1, null ) ) );
            AssertParse ( /*lang=regex*/"a{2}", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, 2 ) ) );
            AssertParse ( /*lang=regex*/"a{2,}", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, null ) ) );
            AssertParse ( /*lang=regex*/"a{2,4}", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, 4 ) ) );
            AssertParse ( /*lang=regex*/"a??", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 0, 1 ) ).Lazily ( ) );
            AssertParse ( /*lang=regex*/"a*?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 0, null ) ).Lazily ( ) );
            AssertParse ( /*lang=regex*/"a+?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 1, null ) ).Lazily ( ) );
            AssertParse ( /*lang=regex*/"a{2}?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, 2 ) ).Lazily ( ) );
            AssertParse ( /*lang=regex*/"a{2,}?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, null ) ).Lazily ( ) );
            AssertParse ( /*lang=regex*/"a{2,4}?", Repetition ( Terminal ( 'a' ), new RepetitionRange ( 2, 4 ) ).Lazily ( ) );
        }

        [TestMethod]
        public void Parse_ParsesSequences ( )
        {
            AssertParse ( /*lang=regex*/@"aaa", Sequence ( Terminal ( 'a' ), Terminal ( 'a' ), Terminal ( 'a' ) ) );
            AssertParse ( /*lang=regex*/@"\d\d\d", Sequence ( CharacterClasses.Digit, CharacterClasses.Digit, CharacterClasses.Digit ) );
            AssertParse ( /*lang=regex*/@"...", Sequence ( CharacterClasses.Dot, CharacterClasses.Dot, CharacterClasses.Dot ) );
        }

        [TestMethod]
        public void Parse_ParsesAlternations ( )
        {
            AssertParse (
                /*lang=regex*/@"a{1,2}|b{1,2}",
                Alternation (
                    Repetition ( Terminal ( 'a' ), new RepetitionRange ( 1, 2 ) ),
                    Repetition ( Terminal ( 'b' ), new RepetitionRange ( 1, 2 ) ) ) );
            AssertParse (
                /*lang=regex*/@"(?:string 1\.1|string 1\.2)|(?:string 2\.1|string 2\.2)",
                Alternation (
                    Alternation (
                        Sequence ( Array.ConvertAll ( "string 1.1".ToCharArray ( ), Terminal ) ),
                        Sequence ( Array.ConvertAll ( "string 1.2".ToCharArray ( ), Terminal ) ) ),
                    Alternation (
                        Sequence ( Array.ConvertAll ( "string 2.1".ToCharArray ( ), Terminal ) ),
                        Sequence ( Array.ConvertAll ( "string 2.2".ToCharArray ( ), Terminal ) ) ) ) );
        }

        [TestMethod]
        public void Parse_ParsesComplexExpressions ( )
        {
            AssertParse (
                /*lang=regex*/@"\[(=*)\[((?!]\1])[\S\s])]\1]",
                Sequence (
                    Terminal ( '[' ),
                    Capture ( 1, Repetition ( Terminal ( '=' ), new RepetitionRange ( 0, null ) ) ),
                    Terminal ( '[' ),
                    Capture (
                        2,
                        Sequence (
                            !PositiveLookahead ( Sequence ( Terminal ( ']' ), Backreference ( 1 ), Terminal ( ']' ) ) ),
                            Set ( CharacterClasses.NotWhitespace, CharacterClasses.Whitespace ) ) ),
                    Terminal ( ']' ),
                    Backreference ( 1 ),
                    Terminal ( ']' ) ) );
        }
    }
}
