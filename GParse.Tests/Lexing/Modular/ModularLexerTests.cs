using System;
using GParse.Errors;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.Lexing.Modular
{
    [TestClass]
    public class ModularLexerTests
    {
        private static void AssertToken<T> ( Token<T> token, String id, T type, String? raw, Object? value, Range<Int32> range )
            where T : notnull
        {
            Assert.AreEqual ( id, token.Id );
            Assert.AreEqual ( type, token.Type );
            Assert.AreEqual ( raw, token.Text );
            Assert.AreEqual ( value, token.Value );
            Assert.AreEqual ( false, token.IsTrivia );
            Assert.AreEqual ( range, token.Range );
        }

        [TestMethod]
        public void LiteralModuleConsumesProperly ( )
        {
            var diagnostics = new DiagnosticList ( );
            var builder = new ModularLexerBuilder<Int32> ( 0 );
            builder.AddLiteral ( "id", 1, "raw" );
            ILexer<Int32> lexer = builder.GetLexer ( "raw", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new Range<Int32> ( 0, 3 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 3 ) );
            lexer = builder.GetLexer ( "rawraw", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new Range<Int32> ( 0, 3 ) );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new Range<Int32> ( 3, 6 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 6 ) );
            lexer = builder.GetLexer ( "notraw", diagnostics );
            Assert.ThrowsException<FatalParsingException> ( ( ) => lexer.Consume ( ), "No registered modules can consume the rest of the input." );
        }

        [TestMethod]
        public void RegexModuleConsumesProperly ( )
        {
            var diagnostics = new DiagnosticList ( );
            var builder = new ModularLexerBuilder<Int32> ( 0 );
            builder.AddRegex ( "id", 1, @"num:(\d+)", "num:", ( m, _ ) => Int32.Parse ( m.Groups[1].Value ), false );

            // Test 01
            ILexer<Int32> lexer = builder.GetLexer ( "num:1", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1", 1, new Range<Int32> ( 0, 5 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 5 ) );

            // Test 02
            lexer = builder.GetLexer ( "num:12", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:12", 12, new Range<Int32> ( 0, 6 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 6 ) );

            // Test 03
            lexer = builder.GetLexer ( "num:1234", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1234", 1234, new Range<Int32> ( 0, 8 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 8 ) );

            // Test 04
            lexer = builder.GetLexer ( "num:1234num:1", diagnostics );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1234", 1234, new Range<Int32> ( 0, 8 ) );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1", 1, new Range<Int32> ( 8, 13 ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, null, null, new Range<Int32> ( 13 ) );

            // Test 05
            lexer = builder.GetLexer ( "num:notnum", diagnostics );
            Assert.ThrowsException<FatalParsingException> ( ( ) => lexer.Consume ( ), "No registered modules can consume the rest of the input." );
        }
    }
}