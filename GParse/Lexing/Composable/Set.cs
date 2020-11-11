using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A regex alternation set.
    /// </summary>
    public sealed class Set : GrammarNode<Char>
    {
        /// <summary>
        /// The characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The ranges matched by this set.
        /// </summary>
        public ImmutableArray<Range<Char>> Ranges { get; }

        /// <summary>
        /// The unicode categories matched by this set.
        /// </summary>
        public ImmutableArray<UnicodeCategory> UnicodeCategories { get; }

        /// <summary>
        /// The nodes matched by this set.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        /// <summary>
        /// The (flattened) character ranges matched by this set.
        /// </summary>
        internal ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories matched by this set.
        /// </summary>
        internal UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// Initializes a new set from its elements.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategories"></param>
        /// <param name="nodes"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="unicodeCategoryBitSet"></param>
        internal Set (
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
        public Set ( params SetElement[] setElements )
        {
            if ( setElements is null )
                throw new ArgumentNullException ( nameof ( setElements ) );
            if ( setElements.Length < 1 )
                throw new ArgumentException ( "At least 1 set element is required.", nameof ( setElements ) );

            ImmutableHashSet<Char>.Builder characters = ImmutableHashSet.CreateBuilder<Char> ( );
            var ranges = new List<Range<Char>> ( );
            ImmutableArray<UnicodeCategory>.Builder categories = ImmutableArray.CreateBuilder<UnicodeCategory> ( );
            ImmutableArray<GrammarNode<Char>>.Builder nodes = ImmutableArray.CreateBuilder<GrammarNode<Char>> ( );
            for ( var elementIdx = 0; elementIdx < setElements.Length; elementIdx++ )
            {
                SetElement setElement = setElements[elementIdx];
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
                        addNode ( setElement.Node );
                        break;
                    case SetElementType.Invalid:
                    default:
                        throw new InvalidOperationException ( $"Invalid set element provided at index {elementIdx}." );
                }
            }

            OptimizationAlgorithms.MergeRanges ( ranges );

            this.Characters = characters.ToImmutable ( );
            this.Ranges = ranges.ToImmutableArray ( );
            this.UnicodeCategories = categories.ToImmutable ( );
            this.UnicodeCategoryFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
            this.FlattenedRanges = CharUtils.FlattenRanges ( ranges );
            this.Nodes = nodes.ToImmutable ( );

            void addNode ( GrammarNode<Char> node )
            {
                switch ( node )
                {
                    case Set set:
                        foreach ( var ch in set.Characters ) characters.Add ( ch );
                        ranges.AddRange ( set.Ranges );
                        categories.AddRange ( set.UnicodeCategories );
                        foreach ( GrammarNode<Char> subNode in set.Nodes ) addNode ( subNode );
                        break;

                    default:
                    {
                        nodes.Add ( node );
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// Negates this set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There is the Negate extension method." )]
        public static NegatedSet operator ! ( Set set )
        {
            if ( set is null )
                throw new ArgumentNullException ( nameof ( set ) );

            return new NegatedSet ( set.Characters, set.Ranges, set.UnicodeCategories, set.Nodes, set.FlattenedRanges, set.UnicodeCategoryFlagSet );
        }
    }
}
