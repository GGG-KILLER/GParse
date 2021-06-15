using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Lexing.Composable;
using GParse.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Tests.Lexing.Composable
{
    [TestClass]
    public class OptimizationAlgorithmsTests
    {
        private static void Shuffle<T>(List<T> list)
        {
            var rng = new Random();
            for (var n = 0; n < list.Count; n++)
            {
                var k = rng.Next(list.Count - 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private static (List<TChar> Characters, List<TRange> Ranges) ParseString<TChar, TRange>(String str, Func<Char, TChar> charConverter, Func<Char, Char, TRange> rangeConverter)
        {
            var parts = str.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts.Where(part => !isPartValid(part)))
                throw new FormatException($"Invalid part format: {part}");
            return (
                parts.Where(part => part.Length == 1).Select(part => charConverter(part[0])).ToList(),
                parts.Where(part => part.Length == 3 && part[1] == '-').Select(part => rangeConverter(part[0], part[2])).ToList()
            );

            static Boolean isPartValid(String part) => part.Length == 1 || (part.Length == 3 && part[1] == '-');
        }

        private static (List<Char>, List<Range<Char>>) Clone(List<Char> characters, List<Range<Char>> ranges) =>
            (new List<Char>(characters), new List<Range<Char>>(ranges));

        private static (List<Char>, List<Range<Char>>) CloneOrdered(List<Char> characters, List<Range<Char>> ranges)
        {
            (List<Char> clonedCharacaters, List<Range<Char>> clonedRanges) = Clone(characters, ranges);
            clonedCharacaters.Sort();
            clonedRanges.Sort();
            return (clonedCharacaters, clonedRanges);
        }

        private static (List<Char>, List<Range<Char>>) CloneReversed(List<Char> characters, List<Range<Char>> ranges)
        {
            (List<Char> clonedCharacters, List<Range<Char>> clonedRanges) = CloneOrdered(characters, ranges);
            clonedCharacters.Reverse();
            clonedRanges.Reverse();
            return (clonedCharacters, clonedRanges);
        }

        private static (List<Char>, List<Range<Char>>) CloneShuffled(List<Char> characters, List<Range<Char>> ranges)
        {
            (List<Char> clonedCharacaters, List<Range<Char>> clonedRanges) = Clone(characters, ranges);
            Shuffle(clonedCharacaters);
            Shuffle(clonedRanges);
            return (clonedCharacaters, clonedRanges);
        }

        private static void CheckAllVariations(
            List<Char> expectedCharacters,
            List<(Char, Char)> expectedRanges,
            List<Char> baseCharacters,
            List<Range<Char>> baseRanges,
            Action<List<Char>, List<Range<Char>>> action)
        {
            // Input in the string order
            {
                // Setup
                (List<Char> characters, List<Range<Char>> ranges) = Clone(baseCharacters, baseRanges);

                // Act
                action(characters, ranges);

                // Check
                (Char, Char)[] rangesAsTuples = ranges.Select(range => (range.Start, range.End)).ToArray();
                CollectionAssert.AreEquivalent(expectedRanges, rangesAsTuples, $"Ranges don't match: {String.Join(", ", rangesAsTuples)}");
                CollectionAssert.AreEquivalent(expectedCharacters, characters, $"Characters don't match: {String.Join(", ", characters)}");
            }

            // Input in order
            {
                // Setup
                (List<Char> characters, List<Range<Char>> ranges) = CloneOrdered(baseCharacters, baseRanges);

                // Act
                action(characters, ranges);

                // Check
                (Char, Char)[] rangesAsTuples = ranges.Select(range => (range.Start, range.End)).ToArray();
                CollectionAssert.AreEquivalent(expectedRanges, rangesAsTuples, $"Ranges don't match. Expected: {String.Join(", ", expectedRanges)} Actual: {String.Join(", ", rangesAsTuples)}");
                CollectionAssert.AreEquivalent(expectedCharacters, characters, $"Characters don't match. Expected: {String.Join(", ", expectedCharacters)} Actual: {String.Join(", ", characters)}");
            }

            // Input in reverse order
            {
                // Setup
                (List<Char> characters, List<Range<Char>> ranges) = CloneReversed(baseCharacters, baseRanges);

                // Act
                action(characters, ranges);

                // Check
                (Char, Char)[] rangesAsTuples = ranges.Select(range => (range.Start, range.End)).ToArray();
                CollectionAssert.AreEquivalent(expectedRanges, rangesAsTuples, $"Ranges don't match. Expected: {String.Join(", ", expectedRanges)} Actual: {String.Join(", ", rangesAsTuples)}");
                CollectionAssert.AreEquivalent(expectedCharacters, characters, $"Characters don't match. Expected: {String.Join(", ", expectedCharacters)} Actual: {String.Join(", ", characters)}");
            }

            // Input in random order
            {
                // Setup
                (List<Char> characters, List<Range<Char>> ranges) = CloneShuffled(baseCharacters, baseRanges);
                Logger.LogMessage("Shuffled input:");
                Logger.LogMessage("Characters: {0}", String.Join("|", characters));
                Logger.LogMessage("Ranges: {0}", String.Join("|", ranges));

                // Act
                action(characters, ranges);

                // Check
                (Char, Char)[] rangesAsTuples = ranges.Select(range => (range.Start, range.End)).ToArray();
                CollectionAssert.AreEquivalent(expectedRanges, rangesAsTuples, $"Ranges don't match. Expected: {String.Join(", ", expectedRanges)} Actual: {String.Join(", ", rangesAsTuples)}");
                CollectionAssert.AreEquivalent(expectedCharacters, characters, $"Characters don't match. Expected: {String.Join(", ", expectedCharacters)} Actual: {String.Join(", ", characters)}");
            }
        }

        [DataTestMethod]
        // Single range
        [DataRow("1|2|3|4|5|6", "1|2|3|4|5|6|1-6")]
        // Multiple ranges
        [DataRow("1|2|3|5|6|7", "1|2|3|5|6|7|1-3|5-7")]
        // Single range and a single spare char
        [DataRow("1|3|4|5|6", "1|3|4|5|6|3-6")]
        // Single range and multiple spare chars
        [DataRow("1|3|4|5|6|8", "1|3|4|5|6|3-6|8")]
        // Multiple ranges and a single spare char
        [DataRow("1|2|3|5|7|8|9", "1|2|3|1-3|5|7|8|9|7-9")]
        // Two large ranges
        [DataRow("a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|0|1|2|3|4|5|6|7|8|9", "a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|a-z|0|1|2|3|4|5|6|7|8|9|0-9")]
        // Multiple ranges with 2 spare chars
        [DataRow("a|b|c|d|e|f|g|h|i|j|k|m|o|p|q|r|s|t|u|w|x|y|z|0|2|3|4|5|6|7|8|9", "a|b|c|d|e|f|g|h|i|j|k|a-k|m|o|p|q|r|s|t|u|o-u|w|x|y|z|w-z|0|2|3|4|5|6|7|8|9|2-9")]
        // No ranges and multiple spare chars
        [DataRow("a|c|e|g|i|k|m|o|q|s|u|w|y|0|2|4|6|8", "a|c|e|g|i|k|m|o|q|s|u|w|y|0|2|4|6|8")]
        public void RangifyCharacters_CreatesRangesProperly(String inputString, String expectedString)
        {
            (List<Char> expectedCharacters, List<(Char, Char)> expectedRanges) = ParseString(expectedString, ch => ch, (start, end) => (start, end));
            (List<Char> baseCharacters, List<Range<Char>> baseRanges) = ParseString(inputString, ch => ch, (start, end) => new Range<Char>(start, end));

            CheckAllVariations(expectedCharacters, expectedRanges, baseCharacters, baseRanges, (characters, ranges) =>
          {
              var res = OptimizationAlgorithms.RangifyCharacters(characters, ranges);

              Assert.AreEqual(inputString != expectedString, res);
          });
        }

        [DataTestMethod]
        // Intersecting ranges
        [DataRow("a-d|g-k|d-g", "a-k")]
        // Adjacent ranges
        [DataRow("a-d|i-m|e-h", "a-m")]
        // Non-mergable ranges
        [DataRow("a-e|g-k", "a-e|g-k")]
        // Adjacent ranges
        [DataRow("n-t|e-h|j-m|a-d", "a-h|j-t")]
        public void MergeRanges_MergesRangesProperly(String providedRanges, String expectedRanges)
        {
            // Setup
            (_, List<Range<Char>> ranges) = ParseString(providedRanges, ch => ch, (start, end) => new Range<Char>(start, end));
            (_, List<(Char, Char)> expected) = ParseString(expectedRanges, ch => ch, (start, end) => (start, end));

            CheckAllVariations(new List<Char>(), expected, new List<Char>(), ranges, (characters, ranges) =>
      {
          var actualReturn = OptimizationAlgorithms.MergeRanges(ranges);

          Assert.AreEqual(providedRanges != expectedRanges, actualReturn);
      });
        }

        [DataTestMethod]
        // Single range and single adjacent character
        [DataRow("a|d|b-c", "a|d|a-d")]
        // Single range and multiple adjacent characters
        [DataRow("a|b|c|f|g|h|d-e", "a|b|c|f|g|h|a-h")]
        // Multiple ranges and single shared adjacent character
        [DataRow("a|b-c|b-g", "a|a-c|a-g")]
        // Single range and no adjacent characters
        [DataRow("a|h|c-f", "a|h|c-f")]
        public void ExpandRanges_ExpandsRangesProperly(String inputString, String expectedString)
        {
            // Setup
            (List<Char> expectedCharacters, List<(Char, Char)> expectedRanges) = ParseString(expectedString, ch => ch, (start, end) => (start, end));
            (List<Char> baseCharacters, List<Range<Char>> baseRanges) = ParseString(inputString, ch => ch, (start, end) => new Range<Char>(start, end));

            CheckAllVariations(expectedCharacters, expectedRanges, baseCharacters, baseRanges, (characters, ranges) =>
          {
              var res = OptimizationAlgorithms.ExpandRanges(characters, ranges);

              Assert.AreEqual(inputString != expectedString, res);
          });
        }
    }
}