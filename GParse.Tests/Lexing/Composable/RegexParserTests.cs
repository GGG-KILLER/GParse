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

        private static void AssertParseThrows ( String pattern, SourceRange range = default, String? message = null )
        {
            RegexParseException exception = Assert.ThrowsException<RegexParseException> ( ( ) => RegexParser.Parse ( pattern ) );

            if ( range != default )
                Assert.AreEqual ( range, exception.Range );
            if ( message is not null )
                Assert.AreEqual ( message, exception.Message );
        }

        [TestMethod]
        public void Parse_ParsesCharacterProperly ( )
        {
            AssertParse ( /*lang=regex*/"a", new CharacterTerminal ( 'a' ) );
            AssertParse ( /*lang=regex*/" ", new CharacterTerminal ( ' ' ) );
        }

        [TestMethod]
        public void Parse_ParsesEscapedCharactersProperly ( )
        {
            AssertParse ( /*lang=regex*/@"\a", new CharacterTerminal ( '\a' ) );
            //AssertParse ( /*lang=regex*/@"\b", new CharacterTerminal ( '\b' ) );
            AssertParse ( /*lang=regex*/@"\f", new CharacterTerminal ( '\f' ) );
            AssertParse ( /*lang=regex*/@"\n", new CharacterTerminal ( '\n' ) );
            AssertParse ( /*lang=regex*/@"\r", new CharacterTerminal ( '\r' ) );
            AssertParse ( /*lang=regex*/@"\t", new CharacterTerminal ( '\t' ) );
            AssertParse ( /*lang=regex*/@"\v", new CharacterTerminal ( '\v' ) );
            AssertParse ( /*lang=regex*/@"\.", new CharacterTerminal ( '.' ) );
            AssertParse ( /*lang=regex*/@"\$", new CharacterTerminal ( '$' ) );
            AssertParse ( /*lang=regex*/@"\^", new CharacterTerminal ( '^' ) );
            AssertParse ( /*lang=regex*/@"\{", new CharacterTerminal ( '{' ) );
            AssertParse ( /*lang=regex*/@"\[", new CharacterTerminal ( '[' ) );
            AssertParse ( /*lang=regex*/@"\(", new CharacterTerminal ( '(' ) );
            AssertParse ( /*lang=regex*/@"\|", new CharacterTerminal ( '|' ) );
            AssertParse ( /*lang=regex*/@"\)", new CharacterTerminal ( ')' ) );
            AssertParse ( /*lang=regex*/@"\*", new CharacterTerminal ( '*' ) );
            AssertParse ( /*lang=regex*/@"\+", new CharacterTerminal ( '+' ) );
            AssertParse ( /*lang=regex*/@"\?", new CharacterTerminal ( '?' ) );
            AssertParse ( /*lang=regex*/@"\\", new CharacterTerminal ( '\\' ) );

            AssertParse ( /*lang=regex*/@"\x0A", new CharacterTerminal ( '\x0A' ) );

            AssertParseThrows ( /*lang=regex*/@"\b", SourceRange.Zero, "Invalid escape sequence." );
            AssertParseThrows ( @"\g", SourceRange.Zero, "Invalid escape sequence." );
        }

        [TestMethod]
        public void Parse_ParsesCharacterClasses ( )
        {
            AssertParse (/*lang=regex*/@".", CharacterClasses.Dot );
            AssertParse (/*lang=regex*/@"\d", CharacterClasses.Digit );
            AssertParse (/*lang=regex*/@"\D", !CharacterClasses.Digit );
            AssertParse (/*lang=regex*/@"\w", CharacterClasses.Word );
            AssertParse (/*lang=regex*/@"\W", !CharacterClasses.Word );
            AssertParse (/*lang=regex*/@"\s", CharacterClasses.Whitespace );
            AssertParse (/*lang=regex*/@"\S", !CharacterClasses.Whitespace );
            foreach ( KeyValuePair<String, GrammarNode<Char>> pair in CharacterClasses.Unicode.AllCategories )
            {
                AssertParse (/*lang=regex*/$@"\p{{{pair.Key}}}", pair.Value );
                AssertParse (/*lang=regex*/$@"\P{{{pair.Key}}}", pair.Value switch
                {
                    CharacterRange range => !range,
                    UnicodeCategoryTerminal unicodeCategoryTerminal => !unicodeCategoryTerminal,
                    Alternation<Char> alternation => alternation.Negate ( ),
                    _ => throw new InvalidOperationException ( $"Character categories shouldn't have a {pair.Value?.GetType ( ).Name ?? "null"} node." )
                } );
            }

            AssertParseThrows (
                @"\p{Unexistent}",
                new SourceLocation ( 1, 1, 0 ).To ( new SourceLocation ( 1, 15, 14 ) ),
                "Invalid unicode class or code block name: Unexistent." );
        }

        [TestMethod]
        public void Parse_ParsesAlternationSets ( )
        {
            AssertParse (/*lang=regex*/@"[abc]", Set ( 'a', 'b', 'c' ) );
            AssertParse (/*lang=regex*/@"[a-z]", Set ( new Range<Char> ( 'a', 'z' ) ) );
        }
    }
}
