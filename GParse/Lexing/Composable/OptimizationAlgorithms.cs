using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            List<CharacterRange> ranges,
            Boolean areCharactersSorted = false )
        {
            // Rangify charset
            if ( characters.Count <= 1 )
            {
                return false;
            }

            if ( !areCharactersSorted )
                characters.Sort ( );

            var @return = false;
            Char start = characters[0],
                end = characters[0];
            var idx = 1;
            while ( true )
            {
            loopStart:
                if ( idx >= characters.Count )
                    break;

                var current = characters[idx];

                if ( end + 1 == current )
                {
                    characters.RemoveAt ( idx );
                    end++;
                    @return = true;
                    goto loopStart;
                }
                else
                {
                    if ( start != end )
                    {
                        characters.RemoveAt ( idx - 1 ); // remove the range start.
                        ranges.Add ( new CharacterRange ( start, end ) );
                        start = end = current;
                        @return = true;
                        goto loopStart;
                    }
                    start = end = current;
                }

                idx++;
            }

            if ( start != end )
            {
                characters.RemoveAt ( idx - 1 ); // remove the range start.
                ranges.Add ( new CharacterRange ( start, end ) );
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
        public static Boolean MergeRanges ( List<CharacterRange> ranges, Boolean areRangesSorted = false )
        {
            if ( !areRangesSorted )
            {
                ranges.Sort ( ( x, y ) =>
                {
                    var cmp = x.Start.CompareTo ( y.Start );
                    if ( cmp == 0 )
                        cmp = x.End.CompareTo ( y.End );
                    return cmp;
                } );
            }

            var @return = false;

            for ( var outerIdx = 0; outerIdx < ranges.Count; outerIdx++ )
            {
                CharacterRange range1 = ranges[outerIdx];
                var innerIdx = outerIdx + 1;
                while ( true )
                {
                innerLoopStart:
                    if ( innerIdx >= ranges.Count )
                        break;
                    CharacterRange range2 = ranges[innerIdx];

                    if ( intersectsWith ( range1.Start, range1.End, range2.Start, range2.End )
                         || System.Math.Abs ( range2.Start - range1.End ) == 1
                         || System.Math.Abs ( range1.Start - range2.End ) == 1 )
                    {
                        ranges.RemoveAt ( innerIdx );
                        range1 = new CharacterRange ( min ( range1.Start, range2.Start ), max ( range1.End, range2.End ) );
                        ranges[outerIdx] = range1;
                        @return = true;
                        goto innerLoopStart;
                    }

                    innerIdx++;
                }
            }

            return @return;

            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Boolean intersectsWith ( Char start1, Char end1, Char start2, Char end2 ) =>
                start1 <= end2 && start2 <= end1;
            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Char min ( Char a, Char b ) =>
                a < b ? a : b;
            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Char max ( Char a, Char b ) =>
                a > b ? a : b;
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
        public static Boolean ExpandRanges ( List<Char> characters, List<CharacterRange> ranges, Boolean areCharactersSorted = false )
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
                    CharacterRange range = ranges[rangeIdx];
                    Char start = range.Start, end = range.End;

                    if ( end < Char.MaxValue && crescentChar == end + 1 )
                    {
                        start = crescentChar;
                    }

                    if ( start > Char.MinValue && start - 1 == decrescentChar )
                    {
                        end = decrescentChar;
                    }

                    if ( range.Start != start || range.End != end )
                    {
                        ranges[rangeIdx] = new CharacterRange ( start, end );
                        @return = true;
                    }
                }
            }

            return @return;
        }
    }
}
