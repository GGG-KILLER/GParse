using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using GParse.Math;

namespace GParse.Utilities
{
    /// <summary>
    /// A class containing utility methods for <see cref="Char"/>s.
    /// </summary>
    internal static class CharUtils
    {
        /// <summary>
        /// Checks whether the provided <paramref name="value" /> is in the range [<paramref
        /// name="start" />, <paramref name="end" />].
        /// </summary>
        /// <param name="start">The first character of the range (inclusive).</param>
        /// <param name="value">The character to check for.</param>
        /// <param name="end">The last character of the range (inclusive).</param>
        /// <returns>Whether the provided character is in the range.</returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsInRange ( Char start, Char value, Char end ) =>
            ( UInt32 ) ( value - start ) <= ( end - start );

        /// <summary>
        /// Converts the provided ASCII character into lower-case ASCII.
        /// </summary>
        /// <param name="ch">The character to convert.</param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Char AsciiLowerCase ( Char ch ) =>
            ( Char ) ( ch | 0b100000 );

        /// <summary>
        /// Converts a character into a readable (possibly escaped) string representation.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static String ToReadableString ( Char ch ) =>
            ch == ' ' || Char.IsLetterOrDigit ( ch ) || Char.IsPunctuation ( ch )
                ? Char.ToString ( ch )
                : $"\\u{( UInt16 ) ch:X4}";

        /// <summary>
        /// Escapes a character for usage in regex.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static String RegexEscape ( Char ch )
        {
            if ( ch is '.' or '$' or '^' or '{' or '[' or '(' or '|' or ')' or '*' or '+' or '?' )
                return $"\\{ch}";
            else
                return ToReadableString ( ch );
        }

        /// <summary>
        /// Sorts and flattens the ranges in the provided list.
        /// </summary>
        /// <param name="ranges">The MERGED AND SORTED ranges list.</param>
        /// <returns></returns>
        public static ImmutableArray<Char> FlattenRanges ( IEnumerable<Range<Char>> ranges )
        {
            var rangesArray = ranges.ToImmutableArray ( );
            ImmutableArray<Char>.Builder flattened = ImmutableArray.CreateBuilder<Char> ( rangesArray.Length << 1 );
            for ( var rangeIdx = 0; rangeIdx < rangesArray.Length; rangeIdx++ )
            {
                Range<Char> range = rangesArray[rangeIdx];
                flattened.Add ( range.Start );
                flattened.Add ( range.End );
            }
            return flattened.MoveToImmutable ( );
        }

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided (sorted and flattened) list.
        /// </summary>
        /// <param name="idx">The index found by binary search.</param>
        /// 
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static Boolean InnerIsInRangesIndexCheck ( Int32 idx ) =>
            // If the next greatest value's index is odd, then the character is in
            // the middle of a range. Since the length is always even, we don't need
            // to worry about the element not being in the array since it'll return 0
            // or an even number which will not pass the odd check.
            idx >= 0 || ( ~idx & 1 ) == 1;

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided SORTED AND FLATTENED range list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
        public static Boolean IsInRanges ( Char[] ranges, Char ch ) =>
            ranges.Length == 2
            ? IsInRange ( ranges[0], ch, ranges[1] )
            : InnerIsInRangesIndexCheck ( Array.BinarySearch ( ranges, ch ) );

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided SORTED AND FLATTENED range list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
        public static Boolean IsInRanges ( ImmutableArray<Char> ranges, Char ch ) =>
            ranges.Length == 2
            ? IsInRange ( ranges[0], ch, ranges[1] )
            : InnerIsInRangesIndexCheck ( ranges.BinarySearch ( ch ) );

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided SORTED AND FLATTENED range list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
        public static Boolean IsInRanges ( List<Char> ranges, Char ch ) =>
            ranges.Count == 2
            ? IsInRange ( ranges[0], ch, ranges[1] )
            : InnerIsInRangesIndexCheck ( ranges.BinarySearch ( ch ) );

        /// <summary>
        /// Creates a flagset from a list of unicode categories.
        /// </summary>
        /// <param name="unicodeCategories"></param>
        /// <returns></returns>
        public static UInt32 CreateCategoryFlagSet ( IEnumerable<UnicodeCategory> unicodeCategories ) =>
            unicodeCategories.Aggregate ( 0U, ( flagSet, unicodeCategory ) => flagSet | ( 1U << ( Int32 ) unicodeCategory ) );

        /// <summary>
        /// Checks if the provided category is in the flagset.
        /// </summary>
        /// <param name="flagSet"></param>
        /// <param name="unicodeCategory"></param>
        /// <returns></returns>
        public static Boolean IsCategoryInSet ( UInt32 flagSet, UnicodeCategory unicodeCategory ) =>
            ( ( 1 << ( Int32 ) unicodeCategory ) & flagSet ) != 0;
    }
}
