using System;
using System.Collections.Generic;
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

        private static void AssertParseThrows ( String pattern, Int32 offsetStart, Int32 offsetEnd, String message )
        {
            RegexParseException exception = Assert.ThrowsException<RegexParseException> ( ( ) => RegexParser.Parse ( pattern ) );

            Assert.AreEqual (
                new SourceRange (
                    SourceLocation.Calculate ( pattern, offsetStart ),
                    SourceLocation.Calculate ( pattern, offsetEnd ) ),
                exception.Range );
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

            AssertParseThrows ( @"\b", 0, 0, "Invalid escape sequence." );
            AssertParseThrows ( @"\g", 0, 0, "Invalid escape sequence." );
        }

        [TestMethod]
        public void Parse_ParsesCharacterClasses ( )
        {
            AssertParse ( /*lang=regex*/@".", CharacterClasses.Dot );
            AssertParse ( /*lang=regex*/@"\d", CharacterClasses.Digit );
            AssertParse ( /*lang=regex*/@"\D", !CharacterClasses.Digit );
            AssertParse ( /*lang=regex*/@"\w", CharacterClasses.Word );
            AssertParse ( /*lang=regex*/@"\W", !CharacterClasses.Word );
            AssertParse ( /*lang=regex*/@"\s", CharacterClasses.Whitespace );
            AssertParse ( /*lang=regex*/@"\S", !CharacterClasses.Whitespace );
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
            AssertParse ( /*lang=regex*/@"[a-z]", Set ( new Range<Char> ( 'a', 'z' ) ) );
            AssertParse ( /*lang=regex*/@"[\d\s]", Set ( CharacterClasses.Digit, CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[^\d\s]", !Set ( CharacterClasses.Digit, CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[^\D\S]", !Set ( !CharacterClasses.Digit, !CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[\d-\s]", Set ( CharacterClasses.Digit, '-', CharacterClasses.Whitespace ) );
            AssertParse ( /*lang=regex*/@"[]]", Set ( ']' ) );
            AssertParse ( /*lang=regex*/@"[^]]", !Set ( ']' ) );
            AssertParseThrows ( @"[]", 0, 2, "Unfinished set." );
            AssertParseThrows ( @"[^]", 0, 3, "Unfinished set." );
        }

        [TestMethod]
        public void Parse_ParsesLookahead ( )
        {
            AssertParse ( /*lang=regex*/@"(?=a)", Lookahead ( Terminal ( 'a' ) ) );
            AssertParse ( /*lang=regex*/@"(?=[\d])", Lookahead ( Set ( CharacterClasses.Digit ) ) );
            AssertParse ( /*lang=regex*/@"(?!a)", !Lookahead ( Terminal ( 'a' ) ) );
            AssertParse ( /*lang=regex*/@"(?![\d])", !Lookahead ( Set ( CharacterClasses.Digit ) ) );

            AssertParseThrows ( @"(?=", 0, 3, "Unfinished lookahead." );
            AssertParseThrows ( @"(?!", 0, 3, "Unfinished lookahead." );
        }

        [TestMethod]
        public void Parse_ParsesNonCapturingGroup ( )
        {
            AssertParse ( /*lang=regex*/@"(?:a)", Terminal ( 'a' ) );
            AssertParse ( /*lang=regex*/@"(?:(?:[\d]))", Set ( CharacterClasses.Digit ) );

            AssertParseThrows ( @"(?:", 0, 3, "Unfinished non-capturing group." );
        }

        [TestMethod]
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
    }
}
