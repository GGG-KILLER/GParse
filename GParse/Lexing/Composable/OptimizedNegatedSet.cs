using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An optimized representation of <see cref="NegatedSet"/>.
    /// Currently can only be created by the <see cref="GrammarTreeOptimizer"/>.
    /// </summary>
    public sealed class OptimizedNegatedSet : GrammarNode<Char>
    {
        /// <summary>
        /// A bitvector of characters not matched by this set..
        /// Available when there are few characters and they're close enough.
        /// </summary>
        public CharacterBitVector? CharaterBitVector { get; }

        /// <summary>
        /// The characters not matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The (flattened) character ranges not matched by this set.
        /// </summary>
        public ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories not matched by this set.
        /// </summary>
        public UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// The nodes not matched by this set.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        /// <summary>
        /// Initializes a new optimized negated set.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="unicodeCategoryFlagSet"></param>
        /// <param name="nodes"></param>
        /// <param name="characterBitVector"></param>
        internal OptimizedNegatedSet (
            IImmutableSet<Char> characters,
            ImmutableArray<Char> flattenedRanges,
            UInt32 unicodeCategoryFlagSet,
            ImmutableArray<GrammarNode<Char>> nodes,
            CharacterBitVector? characterBitVector = null )
        {
            this.Characters = characters;
            this.FlattenedRanges = flattenedRanges;
            this.UnicodeCategoryFlagSet = unicodeCategoryFlagSet;
            this.Nodes = nodes;
            this.CharaterBitVector = characterBitVector;
        }

        /// <summary>
        /// Negated an optimized negated set.
        /// </summary>
        /// <param name="optimizedNegatedSet"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's the Negate extension method." )]
        public static OptimizedSet operator ! ( OptimizedNegatedSet optimizedNegatedSet )
        {
            if ( optimizedNegatedSet is null )
                throw new ArgumentNullException ( nameof ( optimizedNegatedSet ) );
            return new OptimizedSet (
                optimizedNegatedSet.Characters,
                optimizedNegatedSet.FlattenedRanges,
                optimizedNegatedSet.UnicodeCategoryFlagSet,
                optimizedNegatedSet.Nodes,
                optimizedNegatedSet.CharaterBitVector );
        }
    }
}
