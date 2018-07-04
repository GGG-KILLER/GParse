using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Common.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Parsing.Tests
{
    [TestClass]
    public class TokenManagerTests
    {
        private TokenManager TokenMan;
        private SourceCodeReader Reader;

        [TestInitialize]
        public void Init ( )
        {
            this.TokenMan = new TokenManager ( );
            this.TokenMan.AddToken ( "t1", "t1", TokenType.Identifier );
            this.TokenMan.AddToken ( "t2", "t2", TokenType.Identifier, Char.IsWhiteSpace );
            this.Reader = new SourceCodeReader ( "t1t2 t2t1" );
        }

        [TestMethod]
        public void ReadTokenTest ( )
        {
            Token tok = this.TokenMan.ReadToken ( this.Reader );
            Assert.AreEqual ( "t1", tok.ID );
            Assert.AreEqual ( "t1", tok.Raw );
            Assert.AreEqual ( "t1", tok.Value );
            Assert.AreEqual ( TokenType.Identifier, tok.Type );
            Assert.AreEqual ( SourceLocation.Zero.To ( new SourceLocation ( 0, 2, 2 ) ), tok.Range );

            tok = this.TokenMan.ReadToken ( this.Reader );
            Assert.AreEqual ( "t2", tok.ID );
            Assert.AreEqual ( "t2", tok.Raw );
            Assert.AreEqual ( "t2", tok.Value );
            Assert.AreEqual ( TokenType.Identifier, tok.Type );
            Assert.AreEqual ( new SourceLocation ( 0, 2, 2 ).To ( new SourceLocation ( 0, 4, 4 ) ), tok.Range );
            this.Reader.Advance ( 1 );

            Assert.ThrowsException<LexException> ( ( ) => this.TokenMan.ReadToken ( this.Reader ),
                "Failed to find separator for this token." );
        }

        [TestMethod]
        public void TryReadTokenTest ( )
        {
            Assert.IsTrue ( this.TokenMan.TryReadToken ( this.Reader, out Token tok ) );
            Assert.AreEqual ( "t1", tok.ID );
            Assert.AreEqual ( "t1", tok.Raw );
            Assert.AreEqual ( "t1", tok.Value );
            Assert.AreEqual ( TokenType.Identifier, tok.Type );
            Assert.AreEqual ( SourceLocation.Zero.To ( new SourceLocation ( 0, 2, 2 ) ), tok.Range );

            Assert.IsTrue ( this.TokenMan.TryReadToken ( this.Reader, out tok ) );
            Assert.AreEqual ( "t2", tok.ID );
            Assert.AreEqual ( "t2", tok.Raw );
            Assert.AreEqual ( "t2", tok.Value );
            Assert.AreEqual ( TokenType.Identifier, tok.Type );
            Assert.AreEqual ( new SourceLocation ( 0, 2, 2 ).To ( new SourceLocation ( 0, 4, 4 ) ), tok.Range );
            this.Reader.Advance ( 1 );

            Assert.IsFalse ( this.TokenMan.TryReadToken ( this.Reader, out tok ) );
            Assert.IsNull ( tok );
        }
    }
}
