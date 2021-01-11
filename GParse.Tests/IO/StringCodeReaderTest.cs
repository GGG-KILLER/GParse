using System;
using System.Text.RegularExpressions;
using GParse.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.IO
{
    [TestClass]
    public class StringCodeReaderTest
    {
        [TestMethod]
        public void GetLocation__ReturnsCorrectLocation ( )
        {
            var reader = new StringCodeReader ( "stri\nng\r\nanother\r\nstring" );
            var expectedLines = new[]
            {
                /*s :*/ "B 00 L 1 C 1",
                /*t :*/ "B 01 L 1 C 2",
                /*r :*/ "B 02 L 1 C 3",
                /*i :*/ "B 03 L 1 C 4",
                /*\n:*/ "B 04 L 1 C 5",
                /*n :*/ "B 05 L 2 C 1",
                /*g :*/ "B 06 L 2 C 2",
                /*\r:*/ "B 07 L 2 C 3",
                /*\n:*/ "B 08 L 2 C 4",
                /*a :*/ "B 09 L 3 C 1",
                /*n :*/ "B 10 L 3 C 2",
                /*o :*/ "B 11 L 3 C 3",
                /*t :*/ "B 12 L 3 C 4",
                /*h :*/ "B 13 L 3 C 5",
                /*e :*/ "B 14 L 3 C 6",
                /*r :*/ "B 15 L 3 C 7",
                /*\r:*/ "B 16 L 3 C 8",
                /*\n:*/ "B 17 L 3 C 9",
                /*s :*/ "B 18 L 4 C 1",
                /*t :*/ "B 19 L 4 C 2",
                /*r :*/ "B 20 L 4 C 3",
                /*i :*/ "B 21 L 4 C 4",
                /*n :*/ "B 22 L 4 C 5",
                /*g :*/ "B 23 L 4 C 6",
            };
            var i = 0;
            while ( reader.Position != reader.Length )
            {
                SourceLocation l = reader.GetLocation ( );
                reader.Advance ( 1 );
                Assert.AreEqual ( expectedLines[i++], $"B {l.Byte:00} L {l.Line} C {l.Column}" );
            }
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( 1 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( -1 ) );
        }

        [TestMethod]
        public void FindOffset_Char__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( 'b' ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 2 );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( 2, reader.FindOffset ( 'b' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( 'c' ) );
            Assert.AreEqual ( 2, reader.Position );
        }

        [TestMethod]
        public void FindOffset_Char_Int32__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 1, reader.FindOffset ( 'a', 1 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 5, reader.FindOffset ( 'b', 5 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( 'b', 8 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 2 );
            Assert.AreEqual ( -1, reader.FindOffset ( 'a', 1 ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( 3, reader.FindOffset ( 'b', 3 ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( 'c' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( 'b', 6 ) );
            Assert.AreEqual ( 2, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.FindOffset ( 'a', -1 ),
                "The offset must be positivel." );
        }

        [TestMethod]
        public void FindOffset_String__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( "aaa" ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( "bbb" ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( 0, reader.FindOffset ( "aa" ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "aaa" ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( 3, reader.FindOffset ( "bbb" ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "ccc" ) );
            Assert.AreEqual ( 1, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( ( String ) null ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( "" ),
                "String cannot be null or empty." );
        }

        [TestMethod]
        public void FindOffset_String_Int32__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 1, reader.FindOffset ( "aa", 1 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( "bbb", 4 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( -1, reader.FindOffset ( "aa", 1 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "aaa", 1 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( 3, reader.FindOffset ( "bbb", 3 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "ccc", 9 ) );
            Assert.AreEqual ( 1, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( ( String ) null, 1 ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( "", 1 ),
                "String cannot be null or empty." );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.FindOffset ( "a", -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void FindOffset_Predicate__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "a b c d" );
            Assert.AreEqual ( 0, reader.FindOffset ( c => c == 'a' ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 2, reader.FindOffset ( c => c == 'b' ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( c => c == 'c' ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 6, reader.FindOffset ( c => c == 'd' ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 2 );
            Assert.AreEqual ( -1, reader.FindOffset ( c => c == 'a' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( 0, reader.FindOffset ( c => c == 'b' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( 2, reader.FindOffset ( c => c == 'c' ) );
            Assert.AreEqual ( 2, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( c => c == 'd' ) );
            Assert.AreEqual ( 2, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.FindOffset ( ( Predicate<Char> ) null ) );
#nullable restore
        }

        [TestMethod]
        public void FindOffset_Predicate_Int32__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "a b c d a b c d" );
            Assert.AreEqual ( 8, reader.FindOffset ( c => c == 'a', 7 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 10, reader.FindOffset ( c => c == 'b', 7 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 12, reader.FindOffset ( c => c == 'c', 7 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 14, reader.FindOffset ( c => c == 'd', 7 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 7 );
            Assert.AreEqual ( -1, reader.FindOffset ( c => c == 'a', 4 ) );
            Assert.AreEqual ( 7, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( c => c == 'b', 4 ) );
            Assert.AreEqual ( 7, reader.Position );
            Assert.AreEqual ( 5, reader.FindOffset ( c => c == 'c', 4 ) );
            Assert.AreEqual ( 7, reader.Position );
            Assert.AreEqual ( 7, reader.FindOffset ( c => c == 'd', 4 ) );
            Assert.AreEqual ( 7, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.FindOffset ( ( Predicate<Char> ) null, 1 ) );
#nullable restore
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.FindOffset ( c => true, -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void FindOffset_ReadOnlySpan__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( "aaa".AsSpan ( ) ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( "bbb".AsSpan ( ) ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( 0, reader.FindOffset ( "aa".AsSpan ( ) ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "aaa".AsSpan ( ) ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( 3, reader.FindOffset ( "bbb".AsSpan ( ) ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "ccc".AsSpan ( ) ) );
            Assert.AreEqual ( 1, reader.Position );

            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( ReadOnlySpan<Char>.Empty, 1 ),
                "The span must not be empty." );
        }

        [TestMethod]
        public void FindOffset_ReadOnlySpan_Int32__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 1, reader.FindOffset ( "aa".AsSpan ( ), 1 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 4, reader.FindOffset ( "bbb".AsSpan ( ), 4 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( -1, reader.FindOffset ( "aa".AsSpan ( ), 1 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "aaa".AsSpan ( ), 1 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( 3, reader.FindOffset ( "bbb".AsSpan ( ), 3 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( -1, reader.FindOffset ( "ccc".AsSpan ( ), 9 ) );
            Assert.AreEqual ( 1, reader.Position );

            Assert.ThrowsException<ArgumentException> ( ( ) => reader.FindOffset ( ReadOnlySpan<Char>.Empty, 1 ),
                "The span must not be empty." );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.FindOffset ( "a".AsSpan ( ), -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void IsNext_Char__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( 'a' ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( 'b' ) );
            Assert.AreEqual ( 3, reader.Position );
        }

        [TestMethod]
        public void IsNext_String__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( "aa " ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsNext ( "aaa" ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsNext ( "aa bbc" ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( "b" ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.IsTrue ( reader.IsNext ( "bb" ) );
            Assert.AreEqual ( 3, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsNext ( ( String ) null ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsNext ( "" ),
                "String cannot be null or empty." );
        }

        [TestMethod]
        public void IsNext_ReadOnlySpan__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( "aa ".AsSpan ( ) ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsNext ( "aaa".AsSpan ( ) ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsNext ( "aa bbc".AsSpan ( ) ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( "b".AsSpan ( ) ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.IsTrue ( reader.IsNext ( "bb".AsSpan ( ) ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsNext ( ReadOnlySpan<Char>.Empty ),
                "The span must not be empty." );
        }

        [TestMethod]
        public void IsAt_Char__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "abababababababababab" );
            Assert.IsTrue ( reader.IsAt ( 'a', 4 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsTrue ( reader.IsAt ( 'b', 3 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( 'a', 5 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( 'b', 2 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.IsTrue ( reader.IsAt ( 'a', 3 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.IsTrue ( reader.IsAt ( 'b', 4 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.IsFalse ( reader.IsAt ( 'a', 6 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.IsFalse ( reader.IsAt ( 'b', 1 ) );
            Assert.AreEqual ( 1, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.IsAt ( 'a', -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void IsAt_String__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "hi there hi there hi there" );
            Assert.IsTrue ( reader.IsAt ( "hi", 9 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsTrue ( reader.IsAt ( "there", 3 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "hi", 11 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "there", 8 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 11 );
            Assert.IsTrue ( reader.IsAt ( "there", 1 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsTrue ( reader.IsAt ( "hi", 7 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "hi", 11 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "there", 8 ) );
            Assert.AreEqual ( 11, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsAt ( ( String ) null, 1 ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsAt ( "", 1 ),
                "String cannot be null or empty." );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.IsAt ( "a", -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void IsAt_ReadOnlySpan__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "hi there hi there hi there" );
            Assert.IsTrue ( reader.IsAt ( "hi".AsSpan ( ), 9 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsTrue ( reader.IsAt ( "there".AsSpan ( ), 3 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "hi".AsSpan ( ), 11 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "there".AsSpan ( ), 8 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 11 );
            Assert.IsTrue ( reader.IsAt ( "there".AsSpan ( ), 1 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsTrue ( reader.IsAt ( "hi".AsSpan ( ), 7 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "hi".AsSpan ( ), 11 ) );
            Assert.AreEqual ( 11, reader.Position );
            Assert.IsFalse ( reader.IsAt ( "there".AsSpan ( ), 8 ) );
            Assert.AreEqual ( 11, reader.Position );

            Assert.ThrowsException<ArgumentException> ( ( ) => reader.IsAt ( ReadOnlySpan<Char>.Empty, 0 ),
                "The span must not be empty." );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.IsAt ( "a".AsSpan ( ), -1 ),
                "The offset must be positive." );
        }

        [TestMethod]
        public void Peek__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "ab" );
            Assert.AreEqual ( 'a', reader.Peek ( ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'b', reader.Peek ( ) );
            Assert.AreEqual ( 1, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( null, reader.Peek ( ) );
            Assert.AreEqual ( 2, reader.Position );
        }

        [TestMethod]
        public void Peek_Int32__ReturnsCorrectValue ( )
        {
            var reader = new StringCodeReader ( "abc" );
            Assert.AreEqual ( 'b', reader.Peek ( 1 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 'c', reader.Peek ( 2 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( null, reader.Peek ( 20 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'c', reader.Peek ( 1 ) );
            Assert.AreEqual ( 1, reader.Position );
            Assert.AreEqual ( null, reader.Peek ( 2 ) );
            Assert.AreEqual ( 1, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Peek ( -1 ) );
        }

        [TestMethod]
        public void PeekRegex_String__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "123abc" );

            Assert.AreEqual ( "1", reader.PeekRegex ( /*lang=regex*/ @"\d" ).Value );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( "12", reader.PeekRegex ( /*lang=regex*/ @"\d{2}" ).Value );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( "123", reader.PeekRegex ( /*lang=regex*/ @"\d+" ).Value );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.PeekRegex ( /*lang=regex*/ "[a-z]+" ).Success );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsFalse ( reader.PeekRegex ( /*lang=regex*/ @"\d+" ).Success );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "a", reader.PeekRegex ( /*lang=regex*/ @"[a-z]" ).Value );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "ab", reader.PeekRegex ( /*lang=regex*/ @"[a-z]{2}" ).Value );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "abc", reader.PeekRegex ( /*lang=regex*/ @"[a-z]+" ).Value );
            Assert.AreEqual ( 3, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.PeekRegex ( ( String ) null ),
                "'expression' cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.PeekRegex ( "" ),
                "'expression' cannot be null or empty." );
        }

        [TestMethod]
        public void PeekRegex_Regex__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "123abc" );
            var regex1 = new Regex ( @"\G\d+", RegexOptions.Compiled );
            var regex2 = new Regex ( @"\G[a-z]+", RegexOptions.Compiled );
            var regex3 = new Regex ( @"[a-z]+", RegexOptions.Compiled );

            Assert.AreEqual ( "123", reader.PeekRegex ( regex1 ).Value );
            Assert.AreEqual ( 0, reader.Position );
            Assert.IsFalse ( reader.PeekRegex ( regex2 ).Success );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsFalse ( reader.PeekRegex ( regex1 ).Success );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "abc", reader.PeekRegex ( regex2 ).Value );
            Assert.AreEqual ( 3, reader.Position );

            reader.Reset ( );
#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.PeekRegex ( ( Regex ) null ) );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.PeekRegex ( regex3 ),
                "The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location." );
        }

        [TestMethod]
        public void PeekString_Int32__WorksCorrectly ( )
        {
            const String str = "abc abc";
            var reader = new StringCodeReader ( str );

            Assert.AreEqual ( str.Substring ( 0, 3 ), reader.PeekString ( 3 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.AreEqual ( str.Substring ( 3, 4 ), reader.PeekString ( 4 ) );
            Assert.AreEqual ( 3, reader.Position );

            reader.Advance ( 1 );
            Assert.AreEqual ( str.Substring ( 4 ), reader.PeekString ( 4 ) );
            Assert.AreEqual ( 4, reader.Position );
            Assert.AreEqual ( str.Substring ( 4, 3 ), reader.PeekString ( 3 ) );
            Assert.AreEqual ( 4, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekString ( -1 ) );
        }

        [TestMethod]
        public void PeekString_Int32_Int32__WorksCorrectly ( )
        {
            const String str = "abc def ghi";
            var reader = new StringCodeReader ( str );

            Assert.AreEqual ( str.Substring ( 6, 3 ), reader.PeekString ( 3, 6 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( str.Substring ( 4, 3 ), reader.PeekString ( 3, 4 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( str.Substring ( 9 ), reader.PeekString ( 3, 9 ) );
            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( null, reader.PeekString ( 10, 12 ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.AreEqual ( str.Substring ( 6, 3 ), reader.PeekString ( 3, 3 ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( str.Substring ( 5, 3 ), reader.PeekString ( 3, 2 ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( null, reader.PeekString ( 2, 8 ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekString ( -1, 1 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekString ( 1, -1 ) );
        }

        [TestMethod]
        public void PeekSpan_Int32__Test ( )
        {
            const String str = "abc def";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 3 ).Equals ( reader.PeekSpan ( 3 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsTrue ( str.AsSpan ( 3, 4 ).Equals ( reader.PeekSpan ( 4 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekSpan ( -1 ) );
        }

        [TestMethod]
        public void PeekSpan_Int32_Int32__WorksCorrectly ( )
        {
            const String str = "abc def";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 4, 3 ).Equals ( reader.PeekSpan ( 3, 4 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 0, reader.Position );

            reader.Advance ( 3 );
            Assert.IsTrue ( str.AsSpan ( 4, 3 ).Equals ( reader.PeekSpan ( 3, 1 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.IsTrue ( reader.PeekSpan ( 3, 5 ).IsEmpty );
            Assert.AreEqual ( 3, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekSpan ( -1, 1 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekSpan ( 1, -1 ) );
        }

        [TestMethod]
        public void Read__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "abc " );

            Assert.AreEqual ( 0, reader.Position );
            Assert.AreEqual ( 'a', reader.Read ( ) );
            Assert.AreEqual ( 1, reader.Position );

            Assert.AreEqual ( 'c', reader.Read ( 1 ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.AreEqual ( null, reader.Read ( 1 ) );
            Assert.AreEqual ( 3, reader.Position );

            Assert.AreEqual ( ' ', reader.Read ( ) );
            Assert.AreEqual ( 4, reader.Position );

            Assert.AreEqual ( null, reader.Read ( ) );
            Assert.AreEqual ( 4, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Read ( -1 ) );
        }

        [TestMethod]
        public void ReadLine__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "line1\nline2\rline3\r\nline4" );

            Assert.AreEqual ( "line1", reader.ReadLine ( ) );
            Assert.AreEqual ( 6, reader.Position );
            Assert.AreEqual ( "line2", reader.ReadLine ( ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.AreEqual ( "line3", reader.ReadLine ( ) );
            Assert.AreEqual ( 19, reader.Position );
            Assert.AreEqual ( "line4", reader.ReadLine ( ) );
            Assert.AreEqual ( 24, reader.Position );
            Assert.AreEqual ( "", reader.ReadLine ( ) );
            Assert.AreEqual ( 24, reader.Position );
        }

        [TestMethod]
        public void ReadString_Int32__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "abcabc" );

            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( 6, reader.Position );
            Assert.AreEqual ( "", reader.ReadString ( 3 ) );
            Assert.AreEqual ( 6, reader.Position );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.ReadString ( -3 ) );
        }

        [TestMethod]
        public void ReadStringUntil_Char__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aaabbbccc" );
            Assert.AreEqual ( "aaa", reader.ReadStringUntil ( 'b' ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( 'b' ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.AreEqual ( "bbbccc", reader.ReadStringUntil ( 'd' ) );
            Assert.AreEqual ( 9, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( 'b' ) );
            Assert.AreEqual ( 9, reader.Position );
        }

        [TestMethod]
        public void ReadStringUntil_String__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aabbccaaabbbccc" );
            Assert.AreEqual ( "aabbcc", reader.ReadStringUntil ( "aaa" ) );
            Assert.AreEqual ( 6, reader.Position );
            Assert.AreEqual ( "aaabbb", reader.ReadStringUntil ( "ccc" ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( "ccc" ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.AreEqual ( "ccc", reader.ReadStringUntil ( "aaa" ) );
            Assert.AreEqual ( 15, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( "ccc" ) );
            Assert.AreEqual ( 15, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadStringUntil ( ( String ) null ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadStringUntil ( "" ),
                "String cannot be null or empty." );
        }

        [TestMethod]
        public void ReadStringUntil_Predicate__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aaaaa11111aaaaa11111" );
            Assert.AreEqual ( "aaaaa", reader.ReadStringUntil ( Char.IsDigit ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( Char.IsDigit ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.AreEqual ( "11111", reader.ReadStringUntil ( Char.IsLetter ) );
            Assert.AreEqual ( 10, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( Char.IsLetter ) );
            Assert.AreEqual ( 10, reader.Position );
            Assert.AreEqual ( "aaaaa11111", reader.ReadStringUntil ( c => false ) );
            Assert.AreEqual ( 20, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringUntil ( c => false ) );
            Assert.AreEqual ( 20, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.ReadStringUntil ( ( Predicate<Char> ) null ) );
#nullable restore
        }

        [TestMethod]
        public void ReadStringWhile_Predicate__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aaaaa11111aaaaa11111" );
            Assert.AreEqual ( "aaaaa", reader.ReadStringWhile ( Char.IsLetter ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.AreEqual ( "11111aaaaa11111", reader.ReadStringWhile ( Char.IsLetterOrDigit ) );
            Assert.AreEqual ( 20, reader.Position );
            Assert.AreEqual ( "", reader.ReadStringWhile ( Char.IsDigit ) );
            Assert.AreEqual ( 20, reader.Position );
        }

        [TestMethod]
        public void ReadToEnd__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aaaaa" );

            Assert.AreEqual ( "aaaaa", reader.ReadToEnd ( ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.AreEqual ( "", reader.ReadToEnd ( ) );
            Assert.AreEqual ( 5, reader.Position );

            reader.Reset ( );
            reader.Advance ( 2 );

            Assert.AreEqual ( "aaa", reader.ReadToEnd ( ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.AreEqual ( "", reader.ReadToEnd ( ) );
            Assert.AreEqual ( 5, reader.Position );
        }

        [TestMethod]
        public void ReadSpanLine__WorksCorrectly ( )
        {
            const String str = "line1\nline2\rline3\r\nline4";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 5 ).Equals ( reader.ReadSpanLine ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 6, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 6, 5 ).Equals ( reader.ReadSpanLine ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 12, 5 ).Equals ( reader.ReadSpanLine ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 19, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 19, 5 ).Equals ( reader.ReadSpanLine ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 24, reader.Position );
            Assert.IsTrue ( reader.ReadSpanLine ( ).IsEmpty );
            Assert.AreEqual ( 24, reader.Position );
        }

        [TestMethod]
        public void ReadSpan_Int32__WorksCorrectly ( )
        {
            const String str = "aaaabbbbcccc";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 4 ).Equals ( reader.ReadSpan ( 4 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 4, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 4, 4 ).Equals ( reader.ReadSpan ( 4 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 8, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 8, 4 ).Equals ( reader.ReadSpan ( 5 ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.IsTrue ( reader.ReadSpan ( 4 ).IsEmpty );
            Assert.AreEqual ( 12, reader.Position );
        }

        [TestMethod]
        public void ReadSpanUntil_Char__WorksCorrectly ( )
        {
            const String str = "aaabbbccc";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 3 ).Equals ( reader.ReadSpanUntil ( 'b' ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 3, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( 'b' ).IsEmpty );
            Assert.AreEqual ( 3, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 3 ).Equals ( reader.ReadSpanUntil ( 'd' ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 9, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( 'b' ).IsEmpty );
            Assert.AreEqual ( 9, reader.Position );
        }

        [TestMethod]
        public void ReadSpanUntil_String__WorksCorrectly ( )
        {
            const String str = "aabbccaaabbbccc";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 6 ).Equals ( reader.ReadSpanUntil ( "aaa" ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 6, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 6, 6 ).Equals ( reader.ReadSpanUntil ( "ccc" ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 12, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( "ccc" ).IsEmpty );
            Assert.AreEqual ( 12, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 12, 3 ).Equals ( reader.ReadSpanUntil ( "aaa" ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 15, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( "ccc" ).IsEmpty );
            Assert.AreEqual ( 15, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadSpanUntil ( ( String ) null ),
                "String cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.ReadSpanUntil ( "" ),
                "String cannot be null or empty." );
        }

        [TestMethod]
        public void ReadSpanUntil_Predicate__WorksCorrectly ( )
        {
            const String str = "aaaaa11111aaaaa11111";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 5 ).Equals ( reader.ReadSpanUntil ( Char.IsDigit ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( Char.IsDigit ).IsEmpty );
            Assert.AreEqual ( 5, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 5, 5 ).Equals ( reader.ReadSpanUntil ( Char.IsLetter ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 10, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( Char.IsLetter ).IsEmpty );
            Assert.AreEqual ( 10, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 10 ).Equals ( reader.ReadSpanUntil ( c => false ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 20, reader.Position );
            Assert.IsTrue ( reader.ReadSpanUntil ( c => false ).IsEmpty );
            Assert.AreEqual ( 20, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.ReadSpanUntil ( ( Predicate<Char> ) null ) );
#nullable restore
        }

        [TestMethod]
        public void ReadSpanWhile_Predicate__WorksCorrectly ( )
        {
            const String str = "aaaaa11111aaaaa11111";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( 0, 5 ).Equals ( reader.ReadSpanWhile ( Char.IsLetter ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.IsTrue ( str.AsSpan ( 5 ).Equals ( reader.ReadSpanWhile ( Char.IsLetterOrDigit ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 20, reader.Position );
            Assert.IsTrue ( reader.ReadSpanWhile ( Char.IsDigit ).IsEmpty );
            Assert.AreEqual ( 20, reader.Position );
        }

        [TestMethod]
        public void ReadSpanToEnd__WorksCorrectly ( )
        {
            const String str = "aaaaa";
            var reader = new StringCodeReader ( str );

            Assert.IsTrue ( str.AsSpan ( ).Equals ( reader.ReadSpanToEnd ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.IsTrue ( reader.ReadSpanToEnd ( ).IsEmpty );
            Assert.AreEqual ( 5, reader.Position );

            reader.Reset ( );
            reader.Advance ( 2 );

            Assert.IsTrue ( str.AsSpan ( 2 ).Equals ( reader.ReadSpanToEnd ( ), StringComparison.Ordinal ) );
            Assert.AreEqual ( 5, reader.Position );
            Assert.IsTrue ( reader.ReadSpanToEnd ( ).IsEmpty );
            Assert.AreEqual ( 5, reader.Position );
        }

        [TestMethod]
        public void MatchRegex_String__WorksCorrectly ( )
        {
            var reader = new StringCodeReader ( "aaa111aaa111" );

            Assert.AreEqual ( "aaa", reader.MatchRegex ( /*lang=regex*/ @"[a-z]+" ).Value );
            Assert.AreEqual ( 3, reader.Position );

            Assert.AreEqual ( "111", reader.MatchRegex ( /*lang=regex*/ @"\d+" ).Value );
            Assert.AreEqual ( 6, reader.Position );

            Assert.IsFalse ( reader.MatchRegex ( /*lang=regex*/ @"\d+" ).Success );
            Assert.AreEqual ( 6, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.MatchRegex ( ( String ) null ),
                "'expression' cannot be null or empty." );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.MatchRegex ( "" ),
                "'expression' cannot be null or empty." );
        }

        [TestMethod]
        public void MatchRegex_Regex__WorksCorrectly ( )
        {
            var r1 = new Regex ( @"\G\d+" );
            var r2 = new Regex ( @"\G[a-z]+" );
            var r3 = new Regex ( @"\d+" );
            var reader = new StringCodeReader ( "aaa111aaa111" );

            Assert.IsFalse ( reader.MatchRegex ( r1 ).Success );
            Assert.AreEqual ( 0, reader.Position );

            Assert.AreEqual ( "aaa", reader.MatchRegex ( r2 ).Value );
            Assert.AreEqual ( 3, reader.Position );

            Assert.AreEqual ( "111", reader.MatchRegex ( r1 ).Value );
            Assert.AreEqual ( 6, reader.Position );

#nullable disable
            Assert.ThrowsException<ArgumentNullException> ( ( ) => reader.MatchRegex ( ( Regex ) null ) );
#nullable restore
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.MatchRegex ( r3 ),
                "The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location." );
        }
    }
}