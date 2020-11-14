using System;
using System.Linq;
using GParse.Lexing.Composable;
using GParse.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.Lexing.Composable
{
    [TestClass]
    public class CharacterBitVectorTests
    {
        [TestMethod]
        public void Constructor_HandlesEmptyEnumerable ( )
        {
            Assert.ThrowsException<ArgumentException> (
                ( ) => new CharacterBitVector ( Enumerable.Empty<Char> ( ) ),
                "Not enough characters were provided." );
        }

        [TestMethod]
        public void Constructor_HandlesSingleElementsProperly ( ) =>
            _ = new CharacterBitVector ( Enumerable.Repeat ( 'a', 1 ) );

        [TestMethod]
        public void Contains_ChecksProperly ( )
        {
            {
                var range = new Range<Char> ( '0', '9' );
                var vector = new CharacterBitVector ( Enumerable.Range ( '0', '9' - '0' + 1 ).Select ( c => ( Char ) c ) );

                for ( var ch = '\0'; ch <= 127; ch++ )
                {
                    Assert.AreEqual ( range.ValueIn ( ch ), vector.Contains ( ch ) );
                }
            }
            {
                var chars = new[] { 'a', 'c', 'e', 'g', 'i', 'k', 'm', 'o', 'q', 's', 'u', 'w', 'y' };
                var vector = new CharacterBitVector ( chars );
                for ( var ch = 'a'; ch <= 'z'; ch++ )
                    Assert.AreEqual ( chars.Contains ( ch ), vector.Contains ( ch ) );
                for ( var ch = 'A'; ch <= 'Z'; ch++ )
                    Assert.IsFalse ( vector.Contains ( ch ) );
            }
        }
    }
}
