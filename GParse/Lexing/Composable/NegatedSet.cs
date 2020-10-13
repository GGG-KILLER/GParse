using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

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
        /// The ranges not matched by this set.
        /// </summary>
        public ImmutableArray<Range<Char>> Ranges { get; }

        /// <summary>
        /// The unicode categories not matched by this set.
        /// </summary>
        public ImmutableArray<UnicodeCategory> UnicodeCategories { get; }

        /// <summary>
        /// The nodes not matched by this set.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        /// <summary>
        /// The (flattened) character ranges not matched by this set.
        /// </summary>
        internal ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories not matched by this set.
        /// </summary>
        internal UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// Initializes a new negated set from its elements.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategories"></param>
        /// <param name="nodes"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="unicodeCategoryBitSet"></param>
        internal NegatedSet (
            IImmutableSet<Char> characters,
            ImmutableArray<Range<Char>> ranges,
            ImmutableArray<UnicodeCategory> unicodeCategories,
            ImmutableArray<GrammarNode<Char>> nodes,
            ImmutableArray<Char> flattenedRanges,
            UInt32 unicodeCategoryBitSet )
        {
            this.Characters = characters;
            this.Ranges = ranges;
            this.UnicodeCategories = unicodeCategories;
            this.Nodes = nodes;
            this.FlattenedRanges = flattenedRanges;
            this.UnicodeCategoryFlagSet = unicodeCategoryBitSet;
        }

        /// <summary>
        /// Initializes a new alternation set.
        /// </summary>
        /// <param name="setElements">The elements of this set.</param>
        public NegatedSet ( params SetElement[] setElements )
        {
            ImmutableHashSet<Char>.Builder characters = ImmutableHashSet.CreateBuilder<Char> ( );
            var ranges = new List<Range<Char>> ( );
            ImmutableArray<UnicodeCategory>.Builder categories = ImmutableArray.CreateBuilder<UnicodeCategory> ( );
            ImmutableArray<GrammarNode<Char>>.Builder nodes = ImmutableArray.CreateBuilder<GrammarNode<Char>> ( );
            foreach ( SetElement setElement in setElements )
            {
                switch ( setElement.Type )
                {
                    case SetElementType.Character:
                        characters.Add ( setElement.Character );
                        break;
                    case SetElementType.Range:
                        ranges.Add ( setElement.Range );
                        break;
                    case SetElementType.UnicodeCategory:
                        categories.Add ( setElement.UnicodeCategory );
                        break;
                    case SetElementType.Node:
                        nodes.Add ( setElement.Node );
                        break;
                    case SetElementType.Invalid:
                    default:
                        throw new InvalidOperationException ( "Invalid node provided." );
                }
            }

            OptimizationAlgorithms.MergeRanges ( ranges );

            this.Characters = characters.ToImmutable ( );
            this.Ranges = ranges.ToImmutableArray ( );
            this.UnicodeCategoryFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
            this.FlattenedRanges = CharUtils.FlattenRanges ( ranges );
            this.Nodes = nodes.ToImmutable ( );
        }

        /// <summary>
        /// Un-negates this set.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <returns></returns>
        public static Set operator ! ( NegatedSet negatedSet ) =>
            new Set ( negatedSet.Characters, negatedSet.Ranges, negatedSet.UnicodeCategories, negatedSet.Nodes, negatedSet.FlattenedRanges, negatedSet.UnicodeCategoryFlagSet );
    }
}
