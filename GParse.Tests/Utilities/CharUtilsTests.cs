using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GParse.Math;
using GParse.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.Utilities
{
    [TestClass]
    public class CharUtilsTests
    {
        [TestMethod]
        public void FlattenRanges_WorksProperly()
        {
            // Setup
            var rangeLists = new Range<Char>[][]
            {
                Array.Empty<Range<Char>> ( ),
                new Range<Char>[]
                {
                    new ( '0', '9' ),
                },
                new Range<Char>[]
                {
                    new ( '0', '9' ),
                    new ( 'A', 'Z' ),
                },
                new Range<Char>[]
                {
                    new ( '0', '9' ),
                    new ( 'A', 'Z' ),
                    new ( 'a', 'z' ),
                }
            };
            var expectedRangeArrays = new List<ImmutableArray<Char>>
            {
                ImmutableArray.Create<Char> ( ),
                ImmutableArray.Create ( '0', '9' ),
                ImmutableArray.Create ( '0', '9', 'A', 'Z' ),
                ImmutableArray.Create ( '0', '9', 'A', 'Z', 'a', 'z' ),
            };

            // Act
            var obtainedRangeArrays = rangeLists.Select(CharUtils.FlattenRanges).ToList();

            // Check
            for (var arrayIdx = 0; arrayIdx < expectedRangeArrays.Count; arrayIdx++)
            {
                CollectionAssert.AreEqual(expectedRangeArrays[arrayIdx], obtainedRangeArrays[arrayIdx]);
            }
        }
    }
}