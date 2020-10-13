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
    public static class CharUtils
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
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided (sorted and flattened) list.
        /// </summary>
        /// <param name="idx">The index found by binary search.</param>
        /// <param name="length">The length of the collection.</param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining
#if NETCOREAPP3_1 || NET5_0
                      | MethodImplOptions.AggressiveOptimization
#endif
        )]
        private static Boolean InnerIsInRangesImpl ( Int32 idx, Int32 length )
        {
            if ( idx >= 0 )
                return true;
            // If the next greatest value's index is odd, then the character is in
            // the middle of a range.
            idx = ~idx;
            return idx < length && ( idx & 1 ) == 1;
        }

        /// <summary>
        /// Sorts and flattens the ranges in the provided list.
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static ImmutableArray<Char> FlattenRanges ( IEnumerable<Range<Char>> ranges )
        {
            var rangesArray = ranges.OrderBy ( range => range ).ToImmutableArray ( );
            ImmutableArray<Char>.Builder flattened = ImmutableArray.CreateBuilder<Char> ( rangesArray.Length << 1 );
            for ( var rangeIdx = 0; rangeIdx < rangesArray.Length; rangeIdx++ )
            {
                Range<Char> range = rangesArray[rangeIdx];
                flattened[( rangeIdx << 1 ) + 0] = range.Start;
                flattened[( rangeIdx << 1 ) + 1] = range.End;
            }
            return flattened.MoveToImmutable ( );
        }

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided (sorted and flattened) list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean IsInRanges ( Char[] ranges, Char ch ) =>
            InnerIsInRangesImpl ( Array.BinarySearch ( ranges, ch ), ranges.Length );

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided (sorted and flattened) list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean IsInRanges ( ImmutableArray<Char> ranges, Char ch ) =>
            InnerIsInRangesImpl ( ranges.BinarySearch ( ch ), ranges.Length );

        /// <summary>
        /// Checks if the provided character is in the middle of any of the ranges
        /// in the provided (sorted and flattened) list.
        /// </summary>
        /// <param name="ranges">The sorted and flattened list.</param>
        /// <param name="ch">The character to find.</param>
        /// <returns></returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean IsInRanges ( List<Char> ranges, Char ch ) =>
            InnerIsInRangesImpl ( ranges.BinarySearch ( ch ), ch );

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
