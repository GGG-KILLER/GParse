using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GParse.Lexing.Composable
{
    internal static class OptimizationAlgorithms
    {
#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static void RangifyCharacters (
            List<Char> characters,
            List<CharacterRange> ranges )
        {
            // Rangify charset
            if ( characters.Count <= 2 )
            {
                return;
            }

            characters.Sort ( );
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
                    goto loopStart;
                }
                else
                {
                    if ( start != end )
                    {
                        characters.RemoveAt ( idx - 1 ); // remove the range start.
                        ranges.Add ( new CharacterRange ( start, end ) );
                        start = end = current;
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
            }
        }

#if NETCOREAPP3_1 || NET5_0
        [MethodImpl ( MethodImplOptions.AggressiveOptimization )]
#endif
        public static void MergeRanges ( List<CharacterRange> ranges )
        {
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
                         || System.Math.Abs ( range2.Start - range1.End ) == 1 )
                    {
                        ranges.RemoveAt ( innerIdx );
                        range1 = new CharacterRange ( min ( range1.Start, range2.Start ), max ( range1.End, range2.End ) );
                        ranges[outerIdx] = range1;
                        goto innerLoopStart;
                    }

                    innerIdx++;
                }
            }

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
    }
}
