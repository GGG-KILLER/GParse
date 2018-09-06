using System;
using System.Diagnostics;
using GParse.Common;
using GParse.Common.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Parsing.Tests
{
    [TestClass]
    public class LexerTests
    {
        private const Double TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000D;

        [TestMethod]
        public void LexerTest ( )
        {
            var lexer = new TestLexer ( @"if ( something == true & true && false )
{
    return a;
}
else
{
    return _foo | 0xFFFFFFF;
}" );
            var expectedTokens = new Token[]
            {
                new Token ( "If", "if", "if", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "LParen", "(", "(", TokenType.LParen, SourceRange.Zero ),
                new Token ( "Identifier", "something", "something", TokenType.Identifier, SourceRange.Zero ),
                new Token ( "Eq", "==", "==", TokenType.Operator, SourceRange.Zero ),
                new Token ( "True", "true", "true", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "&", "&", "&", TokenType.Operator, SourceRange.Zero ),
                new Token ( "True", "true", "true", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "And", "&&", "&&", TokenType.Operator, SourceRange.Zero ),
                new Token ( "False", "false", "false", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "RParen", ")", ")", TokenType.RParen, SourceRange.Zero ),
                new Token ( "LCurly", "{", "{", TokenType.LCurly, SourceRange.Zero ),
                new Token ( "Return", "return", "return", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "Identifier", "a", "a", TokenType.Identifier, SourceRange.Zero ),
                new Token ( "Semicolon", ";", ";", TokenType.Punctuation, SourceRange.Zero ),
                new Token ( "RCurly", "}", "}", TokenType.RCurly, SourceRange.Zero ),
                new Token ( "Else", "else", "else", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "LCurly", "{", "{", TokenType.LCurly, SourceRange.Zero ),
                new Token ( "Return", "return", "return", TokenType.Keyword, SourceRange.Zero ),
                new Token ( "Identifier", "_foo", "_foo", TokenType.Identifier, SourceRange.Zero ),
                new Token ( "BOr", "|", "|", TokenType.Operator, SourceRange.Zero ),
                new Token ( "Number Literal", "0xFFFFFFF", 268435455L, TokenType.Number, SourceRange.Zero ),
                new Token ( "Semicolon", ";", ";", TokenType.Punctuation, SourceRange.Zero ),
                new Token ( "RCurly", "}", "}", TokenType.RCurly, SourceRange.Zero ),
                new Token ( "Eof", "", "", TokenType.EOF, SourceRange.Zero )
            };

            var i = 0;
            var sw = Stopwatch.StartNew ( );
            foreach ( Token token in lexer.Lex ( ) )
            {
                Token expected = expectedTokens[i];
                Logger.LogMessage ( "Expected {0} and got {1}", expected, token );
                Assert.AreEqual ( expected.ID, token.ID );
                Assert.AreEqual ( expected.Raw, token.Raw );
                Assert.AreEqual ( expected.Value, token.Value );
                Assert.AreEqual ( expected.Type, token.Type );
                i++;
            }
            sw.Stop ( );
            Logger.LogMessage ( $"Time elapsed lexing: {sw.ElapsedTicks / TicksPerMicrosecond}μs" );
        }
    }
}
