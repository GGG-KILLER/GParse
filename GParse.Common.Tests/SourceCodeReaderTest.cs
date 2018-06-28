using System;
using GParse.Common.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Common.Tests
{
    [TestClass]
    public class SourceCodeReaderTest
    {
        [TestMethod]
        public void AdvanceTest ( )
        {
            var reader = new SourceCodeReader ( "abc " );
            reader.Advance ( 2 );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( new SourceLocation ( 0, 2, 2 ), reader.Location );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.Advance ( -1 ) );
        }

        [TestMethod]
        public void EOFTest ( )
        {
            var reader = new SourceCodeReader ( "" );
            Assert.IsTrue ( reader.EOF ( ) );

            reader = new SourceCodeReader ( "aaaaaaaaaaaaaaaaaaaa" );
            Assert.IsFalse ( reader.EOF ( ) );
        }

        [TestMethod]
        public void IsNextTest ( )
        {
            var reader = new SourceCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( "aa " ) );
            Assert.IsFalse ( reader.IsNext ( "aaa" ) );
            Assert.IsFalse ( reader.IsNext ( "aa bbc" ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( "b" ) );
            Assert.IsTrue ( reader.IsNext ( "bb" ) );
        }

        [TestMethod]
        public void OffsetOfTest ( )
        {
            var reader = new SourceCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.OffsetOf ( 'a' ) );
            Assert.AreEqual ( 4, reader.OffsetOf ( 'b' ) );

            reader.Advance ( 2 );
            Assert.AreEqual ( 2, reader.OffsetOf ( 'b' ) );
            Assert.AreEqual ( -1, reader.OffsetOf ( 'c' ) );
        }

        [TestMethod]
        public void PeekTest ( )
        {
            var reader = new SourceCodeReader ( "ab" );
            Assert.AreEqual ( 'a', reader.Peek ( ) );
            Assert.AreEqual ( 'b', reader.Peek ( 1 ) );
            Assert.AreEqual ( -1, reader.Peek ( 2 ) );
            Assert.AreEqual ( -1, reader.Peek ( 20 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'b', reader.Peek ( ) );
            Assert.AreEqual ( -1, reader.Peek ( 1 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( -1, reader.Peek ( ) );

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
        public void ReadCharTest ( )
        {
            var reader = new SourceCodeReader ( "abc " );
            Assert.AreEqual ( 'a', reader.ReadChar ( ) );
            Assert.AreEqual ( 'b', reader.ReadChar ( ) );
            Assert.AreEqual ( 'c', reader.ReadChar ( ) );
            Assert.AreEqual ( ' ', reader.ReadChar ( ) );
            Assert.AreEqual ( -1, reader.ReadChar ( ) );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadChar ( -1 ) );
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

        [TestMethod]
        public void SaveAndLoadTest ( )
        {
            var reader = new SourceCodeReader ( "abc " );
            reader.Advance ( reader.Length - 1 );
            reader.Save ( );
            Assert.AreEqual ( ' ', reader.ReadChar ( ) );
            Assert.AreEqual ( reader.Length, reader.Position );

            reader.LoadSave ( );
            Assert.AreEqual ( reader.Length - 1, reader.Position );
            Assert.AreEqual ( ' ', reader.ReadChar ( ) );
            Assert.AreEqual ( -1, reader.ReadChar ( ) );
        }
    }
}
