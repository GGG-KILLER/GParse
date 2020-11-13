﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An optimized form of <see cref="Set"/>.
    /// Currently can only be created by the <see cref="GrammarTreeOptimizer"/>.
    /// </summary>
    public sealed class OptimizedSet : GrammarNode<Char>
    {
        /// <summary>
        /// A bitvector of characters matched by this set..
        /// Available when there are few characters and they're close enough.
        /// </summary>
        public CharacterBitVector? CharaterBitVector { get; }

        /// <summary>
        /// A bitvector of cahracters not matched by this set.
        /// Available when there are few characters and they're close enough.
        /// </summary>
        public CharacterBitVector? NegatedCharacterBitVector { get; }

        /// <summary>
        /// The characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The characters not matched by this set.
        /// </summary>
        public IImmutableSet<Char> NegatedCharacters { get; }

        /// <summary>
        /// The (flattened) character ranges matched by this set.
        /// </summary>
        public ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The (flattened) negated character ranges not matched by this set.
        /// </summary>
        public ImmutableArray<Char> NegatedFlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories matched by this set.
        /// </summary>
        public UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// The flagset of the negated unicode categories matched by this set.
        /// </summary>
        public UInt32 NegatedUnicodeCategoryFlagSet { get; }

        /// <summary>
        /// The nodes that the set matches.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        /// <summary>
        /// Initializes a new optimized set.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="negatedCharacters"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="negatedFlattenedRanges"></param>
        /// <param name="unicodeCategoryFlagSet"></param>
        /// <param name="negatedUnicodeCategoryFlagSet"></param>
        /// <param name="nodes"></param>
        /// <param name="characterBitVector"></param>
        /// <param name="negatedCharacterBitVector"></param>
        internal OptimizedSet (
            IImmutableSet<Char> characters,
            IImmutableSet<Char> negatedCharacters,
            ImmutableArray<Char> flattenedRanges,
            ImmutableArray<Char> negatedFlattenedRanges,
            UInt32 unicodeCategoryFlagSet,
            UInt32 negatedUnicodeCategoryFlagSet,
            ImmutableArray<GrammarNode<Char>> nodes,
            CharacterBitVector? characterBitVector = null,
            CharacterBitVector? negatedCharacterBitVector = null )
        {
            this.Characters = characters;
            this.NegatedCharacters = negatedCharacters;
            this.FlattenedRanges = flattenedRanges;
            this.NegatedFlattenedRanges = negatedFlattenedRanges;
            this.UnicodeCategoryFlagSet = unicodeCategoryFlagSet;
            this.NegatedUnicodeCategoryFlagSet = negatedUnicodeCategoryFlagSet;
            this.Nodes = nodes;
            this.CharaterBitVector = characterBitVector;
            this.NegatedCharacterBitVector = negatedCharacterBitVector;
        }


        /// <summary>
        /// Negated an optimized set.
        /// </summary>
        /// <param name="optimizedSet"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's the Negate extension method." )]
        public static OptimizedNegatedSet? operator ! ( OptimizedSet? optimizedSet ) =>
            optimizedSet is null
                ? null
                : new OptimizedNegatedSet (
                    optimizedSet.Characters,
                    optimizedSet.NegatedCharacters,
                    optimizedSet.FlattenedRanges,
                    optimizedSet.NegatedFlattenedRanges,
                    optimizedSet.UnicodeCategoryFlagSet,
                    optimizedSet.NegatedUnicodeCategoryFlagSet,
                    optimizedSet.Nodes,
                    optimizedSet.CharaterBitVector,
                    optimizedSet.NegatedCharacterBitVector );
    }
}