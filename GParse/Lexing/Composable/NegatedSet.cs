using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A negated regex alternation set.
    /// </summary>
    public sealed class NegatedSet : GrammarNode<Char>
    {
        /// <summary>
        /// The characters not matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The (flattened) character ranges not matched by this set.
        /// </summary>
        public ImmutableArray<Char> Ranges { get; }

        /// <summary>
        /// The bitset of the unicode categories not matched by this set.
        /// </summary>
        public UInt32 UnicodeCategoryBitSet { get; }

        /// <summary>
        /// Initializes a new negated set from its elements.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategoryBitSet"></param>
        internal NegatedSet ( IImmutableSet<Char> characters, ImmutableArray<Char> ranges, UInt32 unicodeCategoryBitSet )
        {
            this.Characters = characters;
            this.Ranges = ranges;
            this.UnicodeCategoryBitSet = unicodeCategoryBitSet;
        }

        /// <summary>
        /// Initializes a new alternation set.
        /// </summary>
        /// <param name="setElements">The elements of this set.</param>
        public NegatedSet ( params SetElement[] setElements )
        {
            this.Characters = setElements.Where ( elem => elem.Type == SetElementType.Character )
                                         .Select ( elem => elem.Character )
                                         .ToImmutableHashSet ( );
            this.UnicodeCategoryBitSet = setElements.Where ( elem => elem.Type == SetElementType.UnicodeCategory )
                                                    .Select ( elem => elem.UnicodeCategory )
                                                    .Aggregate ( 0U, ( acc, category ) => acc | ( 1U << ( Int32 ) category ) );

            var rangesList = new List<Range<Char>> ( setElements.Where ( elem => elem.Type == SetElementType.Range )
                                                                .Select ( elem => elem.Range ) );
            OptimizationAlgorithms.MergeRanges ( rangesList );

            ImmutableArray<Char>.Builder rangesArray = ImmutableArray.CreateBuilder<Char> ( rangesList.Count * 2 );
            for ( var rangeIdx = 0; rangeIdx < rangesList.Count; rangeIdx++ )
            {
                Range<Char> range = rangesList[rangeIdx];
                rangesArray[( rangeIdx << 1 ) + 0] = range.Start;
                rangesArray[( rangeIdx << 1 ) + 1] = range.End;
            }
            this.Ranges = rangesArray.MoveToImmutable ( );
        }

        /// <summary>
        /// Un-negates this set.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <returns></returns>
        public static Set operator ! ( NegatedSet negatedSet ) =>
            new Set ( negatedSet.Characters, negatedSet.Ranges, negatedSet.UnicodeCategoryBitSet );
    }
}
