using System;
using GParse.IO;
using GParse.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.IO
{
    [TestClass]
    public class SourceCodeReaderTest
    {
        [TestMethod]
        public void AdvanceTest ( )
        {
            var reader = new SourceCodeReader ( "stri\nng\n" );
            var expectedLines = new[]
            {
                "B 0 L 1 C 1 - s",
                "B 1 L 1 C 2 - t",
                "B 2 L 1 C 3 - r",
                "B 3 L 1 C 4 - i",
                "B 4 L 1 C 5 - \n",
                "B 5 L 2 C 1 - n",
                "B 6 L 2 C 2 - g",
                "B 7 L 2 C 3 - \n"
            };
            var i = 0;
            while ( reader.HasContent )
            {
                SourceLocation l = reader.Location;
                var c = ( Char ) reader.Read ( );
                Assert.AreEqual ( expectedLines[i++], $"B {l.Byte} L {l.Line} C {l.Column} - {c}" );
            }
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( 1 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( -1 ) );
        }

        [TestMethod]
        public void EOFTest ( )
        {
            var reader = new SourceCodeReader ( "" );
            Assert.IsTrue ( reader.IsAtEOF );
            Assert.IsFalse ( reader.HasContent );

            reader = new SourceCodeReader ( "aaaaaaaaaaaaaaaaaaaa" );
            Assert.IsFalse ( reader.IsAtEOF );
            Assert.IsTrue ( reader.HasContent );
        }

        [TestMethod]
        public void IsNextTest ( )
        {
            var reader = new SourceCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( 'a' ) );
            Assert.IsTrue ( reader.IsNext ( "aa " ) );
            Assert.IsFalse ( reader.IsNext ( "aaa" ) );
            Assert.IsFalse ( reader.IsNext ( "aa bbc" ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( 'b' ) );
            Assert.IsTrue ( reader.IsNext ( "b" ) );
            Assert.IsTrue ( reader.IsNext ( "bb" ) );
        }

        [TestMethod]
        public void OffsetOfTest ( )
        {
            var reader = new SourceCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 4, reader.FindOffset ( 'b' ) );

            reader.Advance ( 2 );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 2, reader.FindOffset ( 'b' ) );
            Assert.AreEqual ( -1, reader.FindOffset ( 'c' ) );
        }

        [TestMethod]
        public void PeekTest ( )
        {
            var reader = new SourceCodeReader ( "ab" );
            Assert.AreEqual ( 'a', reader.Peek ( ) );
            Assert.AreEqual ( 'b', reader.Peek ( 1 ) );
            Assert.AreEqual ( null, reader.Peek ( 2 ) );
            Assert.AreEqual ( null, reader.Peek ( 20 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'b', reader.Peek ( ) );
            Assert.AreEqual ( null, reader.Peek ( 1 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( null, reader.Peek ( ) );

            Assert.ThrowsException<ArgumentException> ( ( ) => reader.Peek ( -1 ) );
        }

        [TestMethod]
        public void PeekStringTest ( )
        {
            var reader = new SourceCodeReader ( "abc abc" );
            Assert.AreEqual ( "abc", reader.PeekString ( 3 ) );

            reader.Advance ( 3 );
            Assert.AreEqual ( " abc", reader.PeekString ( 4 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( null, reader.PeekString ( 4 ) );
            Assert.AreEqual ( "abc", reader.PeekString ( 3 ) );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.PeekString ( -1 ) );
        }

        [TestMethod]
        public void ReadTest ( )
        {
            var reader = new SourceCodeReader ( "abc " );
            Assert.AreEqual ( 'a', reader.Read ( ) );
            Assert.AreEqual ( 'c', reader.Read ( 1 ) );
            Assert.AreEqual ( null, reader.Read ( 1 ) );
            Assert.AreEqual ( ' ', reader.Read ( ) );
            Assert.AreEqual ( null, reader.Read ( ) );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.Read ( -1 ) );
        }

        [TestMethod]
        public void ReadStringTest ( )
        {
            var reader = new SourceCodeReader ( "abcabc" );
            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( null, reader.ReadString ( 3 ) );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadString ( -3 ) );
        }
    }
}
