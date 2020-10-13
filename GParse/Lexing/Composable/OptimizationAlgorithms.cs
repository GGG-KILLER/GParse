using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    internal static class OptimizationAlgorithms
    {
        /// <summary>
        /// Attempts to create ranges from sequential characters.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="areCharactersSorted">Whether the provided character list is sorted.</param>
        /// <returns>Whether any changes were made.</returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean RangifyCharacters (
            List<Char> characters,
            List<Range<Char>> ranges,
            Boolean areCharactersSorted = false )
        {
            if ( characters.Count <= 1 )
                return false;

            if ( !areCharactersSorted )
                characters.Sort ( );

            var @return = false;
            Char start = characters[0],
                 end = characters[0];
            for ( var idx = 1; idx < characters.Count; idx++ )
            {
                var current = characters[idx];

                if ( end + 1 == current )
                {
                    end++;
                    @return = true;
                }
                else
                {
                    if ( start != end )
                    {
                        ranges.Add ( new Range<Char> ( start, end ) );
                        @return = true;
                    }
                    start = end = current;
                }
            }

            if ( start != end )
            {
                ranges.Add ( new Range<Char> ( start, end ) );
                @return = true;
            }

            return @return;
        }

        /// <summary>
        /// Attempts to merge intersecting and adjacent ranges into one.
        /// </summary>
        /// <param name="ranges"></param>
        /// <param name="areRangesSorted">Whether the range list is sorted.</param>
        /// <returns>Whether any changes were made.</returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean MergeRanges ( List<Range<Char>> ranges, Boolean areRangesSorted = false )
        {
            if ( !areRangesSorted )
            {
                ranges.Sort ( );
            }

            var @return = false;

            for ( var outerIdx = 0; outerIdx < ranges.Count; outerIdx++ )
            {
                Range<Char> range1 = ranges[outerIdx];
                var innerIdx = outerIdx + 1;
                while ( true )
                {
                innerLoopStart:
                    if ( innerIdx >= ranges.Count )
                        break;
                    Range<Char> range2 = ranges[innerIdx];

                    if ( range1.IntersectsWith ( range2 ) || range1.IsNeighbourOf ( range2 ) )
                    {
                        ranges.RemoveAt ( innerIdx );
                        range1 = range1.JoinWith ( range2 );
                        ranges[outerIdx] = range1;
                        @return = true;
                        goto innerLoopStart;
                    }

                    innerIdx++;
                }
            }

            return @return;
        }

        /// <summary>
        /// Attempts to expand existing ranges with adjacent characters and removes characters contained in the ranges.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="areCharactersSorted">Whether the character list is sorted.</param>
        /// <returns>Whether any changes were made.</returns>
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static Boolean ExpandRanges (
            List<Char> characters,
            List<Range<Char>> ranges,
            Boolean areCharactersSorted = false )
        {
            if ( !areCharactersSorted )
                characters.Sort ( );

            var @return = false;

            // Ranges' starts can only decrease so we iterate the chars in decrescent order for them
            // and the ranges' end can only increase so we iterate the chars in crecent order for them.
            for ( Int32 crescentCharIdx = 0, decrescentCharIdx = characters.Count - 1;
                  crescentCharIdx < characters.Count && decrescentCharIdx >= 0;
                  crescentCharIdx++, decrescentCharIdx-- )
            {
                var crescentChar = characters[crescentCharIdx];
                var decrescentChar = characters[decrescentCharIdx];

                for ( var rangeIdx = 0; rangeIdx < ranges.Count; rangeIdx++ )
                {
                    Range<Char> range = ranges[rangeIdx];
                    Char start = range.Start, end = range.End;

                    if ( end < Char.MaxValue && crescentChar == end + 1 )
                    {
                        end = crescentChar;
                    }

                    if ( start > Char.MinValue && start - 1 == decrescentChar )
                    {
                        start = decrescentChar;
                    }

                    if ( range.Start != start || range.End != end )
                    {
                        ranges[rangeIdx] = new Range<Char> ( start, end );
                        @return = true;
                    }
                }
            }

            return @return;
        }

        /// <summary>
        /// Removes all characters that are matched by a range or category.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="unicodeCategoryFlagSet"></param>
        public static Boolean RemoveMatchedCharacters (
            List<Char> characters,
            ImmutableArray<Char> flattenedRanges,
            UInt32 unicodeCategoryFlagSet )
        {
            return characters.RemoveAll ( ch => CharUtils.IsInRanges ( flattenedRanges, ch )
                || CharUtils.IsCategoryInSet ( unicodeCategoryFlagSet, Char.GetUnicodeCategory ( ch ) ) ) > 0;
        }
    }
}
