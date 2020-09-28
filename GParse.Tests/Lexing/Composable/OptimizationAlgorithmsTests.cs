using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Lexing.Composable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.Lexing.Composable
{
    [TestClass]
    public class OptimizationAlgorithmsTests
    {
        [DataTestMethod]
        [DataRow ( "123456", "1-6" )]
        [DataRow ( "123", "1-3" )]
        [DataRow ( "123567", "1-3|5-7" )]
        [DataRow ( "13456", "1|3-6" )]
        [DataRow ( "134568", "1|3-6|8" )]
        [DataRow ( "1235789", "1-3|5|7-9" )]
        [DataRow ( "abcdefghijklmnopqrstuvwxyz0123456789", "a-z|0-9" )]
        [DataRow ( "abcdefghijkmopqrstuwxyz023456789", "a-k|m|o-u|w-z|0|2-9" )]
        [DataRow ( "acegikmoqsuwy02468", "a|c|e|g|i|k|m|o|q|s|u|w|y|0|2|4|6|8" )]
        public void RangifyCharacters_CreatesRangesProperly ( String characterSetString, String resultString )
        {
            // Setup
            var parts = resultString.Split ( '|' );
            var expectedCharacters = parts.Where ( s => s.Length == 1 )
                                          .Select ( s => s[0] )
                                          .ToArray ( );
            (Char, Char)[] expectedRanges = parts.Where ( s => s.Length == 3 )
                                                 .Select ( s => (s[0], s[2]) )
                                                 .ToArray ( );
            var characters = characterSetString.ToList ( );
            var ranges = new List<CharacterRange> ( );

            // Act
            OptimizationAlgorithms.RangifyCharacters ( characters, ranges );

            // Check
            (Char, Char)[] outRangesAsTuples = ranges.Select ( range => (range.Start, range.End) ).ToArray ( );
            CollectionAssert.AreEquivalent ( expectedRanges, outRangesAsTuples, $"Ranges don't match: {String.Join ( ", ", outRangesAsTuples )}" );
            CollectionAssert.AreEquivalent ( expectedCharacters, characters, $"Characters don't match: {String.Join ( ", ", characters )}" );
        }

        [DataTestMethod]
        [DataRow ( "a-d|d-g|g-k", "a-k" )]
        [DataRow ( "a-d|e-h|i-m", "a-m" )]
        [DataRow ( "a-e|g-k", "a-e|g-k" )]
        [DataRow ( "a-d|e-h|j-m|n-t", "a-h|j-t" )]
        public void MergeRanges_MergesRangesProperly ( String providedRanges, String expectedRanges )
        {
            // Setup
            var ranges = providedRanges.Split ( '|' )
                                       .Select ( s => new CharacterRange ( s[0], s[2] ) )
                                       .ToList ( );
            var expected = expectedRanges.Split ( '|' )
                                         .Select ( s => (s[0], s[2]) )
                                         .ToList ( );

            // Act
            OptimizationAlgorithms.MergeRanges ( ranges );

            // Check
            (Char, Char)[] tupleRanges = ranges.Select ( range => (range.Start, range.End) ).ToArray ( );
            CollectionAssert.AreEquivalent ( expected, tupleRanges, $"Ranges don't match: {String.Join ( ", ", tupleRanges )}" );
        }
    }
}
