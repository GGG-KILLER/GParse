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
    /// <remarks>
    /// This node matches if a character fits <b>any</b> of this conditions:
    /// <list type="bullet">
    /// <item>Is in the set's <see cref="Characters"/>;</item>
    /// <item>Is not in the set's <see cref="NegatedCharacters"/>;</item>
    /// <item>Is in one of the the set's <see cref="Ranges"/>;</item>
    /// <item>Is not in the set's <see cref="NegatedRanges"/>;</item>
    /// <item>Is in one of the set's <see cref="UnicodeCategories"/>;</item>
    /// <item>Or is not one of the set's <see cref="NegatedUnicodeCategories"/>.</item>
    /// </list>'
    /// </remarks>
    [SuppressMessage ( "Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>" )]
    public sealed class Set : GrammarNode<Char>
    {
        /// <summary>
        /// The characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The negated characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> NegatedCharacters { get; }

        /// <summary>
        /// The ranges matched by this set.
        /// </summary>
        public ImmutableArray<Range<Char>> Ranges { get; }

        /// <summary>
        /// The negated ranges matched by this set.
        /// </summary>
        public ImmutableArray<Range<Char>> NegatedRanges { get; }

        /// <summary>
        /// The unicode categories matched by this set.
        /// </summary>
        public ImmutableArray<UnicodeCategory> UnicodeCategories { get; }

        /// <summary>
        /// The negated unicode categories matched by this set.
        /// </summary>
        public ImmutableArray<UnicodeCategory> NegatedUnicodeCategories { get; }

        /// <summary>
        /// The nodes matched by this set.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        #region Optimized forms

        /// <summary>
        /// The (flattened) character ranges matched by this set.
        /// </summary>
        internal ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The (flattened) negated character ranges not matched by this set.
        /// </summary>
        internal ImmutableArray<Char> NegatedFlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories matched by this set.
        /// </summary>
        internal UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// The flagset of the negated unicode categories matched by this set.
        /// </summary>
        internal UInt32 NegatedUnicodeCategoryFlagSet { get; }

        #endregion Optimized forms

        /// <summary>
        /// Initializes a new set from its components.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="negatedCharacters"></param>
        /// <param name="ranges"></param>
        /// <param name="negatedRanges"></param>
        /// <param name="unicodeCategories"></param>
        /// <param name="negatedUnicodeCategories"></param>
        /// <param name="nodes"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="negatedFlattenedRanges"></param>
        /// <param name="unicodeCategoryFlagSet"></param>
        /// <param name="negatedUnicodeCategoryFlagSet"></param>
        internal Set (
            IImmutableSet<Char> characters,
            IImmutableSet<Char> negatedCharacters,
            ImmutableArray<Range<Char>> ranges,
            ImmutableArray<Range<Char>> negatedRanges,
            ImmutableArray<UnicodeCategory> unicodeCategories,
            ImmutableArray<UnicodeCategory> negatedUnicodeCategories,
            ImmutableArray<GrammarNode<Char>> nodes,
            ImmutableArray<Char> flattenedRanges,
            ImmutableArray<Char> negatedFlattenedRanges,
            UInt32 unicodeCategoryFlagSet,
            UInt32 negatedUnicodeCategoryFlagSet )
        {
            this.Characters = characters ?? throw new ArgumentNullException ( nameof ( characters ) );
            this.NegatedCharacters = negatedCharacters ?? throw new ArgumentNullException ( nameof ( negatedCharacters ) );
            this.Ranges = ranges;
            this.NegatedRanges = negatedRanges;
            this.UnicodeCategories = unicodeCategories;
            this.NegatedUnicodeCategories = negatedUnicodeCategories;
            this.Nodes = nodes;
            this.FlattenedRanges = flattenedRanges;
            this.NegatedFlattenedRanges = negatedFlattenedRanges;
            this.UnicodeCategoryFlagSet = unicodeCategoryFlagSet;
            this.NegatedUnicodeCategoryFlagSet = negatedUnicodeCategoryFlagSet;
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
            ImmutableHashSet<Char>.Builder negatedCharacters = ImmutableHashSet.CreateBuilder<Char> ( );
            var ranges = new List<Range<Char>> ( );
            var negatedRanges = new List<Range<Char>> ( );
            ImmutableArray<UnicodeCategory>.Builder categories = ImmutableArray.CreateBuilder<UnicodeCategory> ( );
            ImmutableArray<UnicodeCategory>.Builder negatedCategories = ImmutableArray.CreateBuilder<UnicodeCategory> ( );
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
            OptimizationAlgorithms.MergeRanges ( negatedRanges );

            this.Characters = characters.ToImmutable ( );
            this.NegatedCharacters = negatedCharacters.ToImmutable ( );
            this.Ranges = ranges.ToImmutableArray ( );
            this.NegatedRanges = negatedRanges.ToImmutableArray ( );
            this.UnicodeCategories = categories.ToImmutable ( );
            this.NegatedUnicodeCategories = negatedCategories.ToImmutable ( );
            this.UnicodeCategoryFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
            this.NegatedUnicodeCategoryFlagSet = CharUtils.CreateCategoryFlagSet ( negatedCategories );
            this.FlattenedRanges = CharUtils.FlattenRanges ( ranges );
            this.NegatedFlattenedRanges = CharUtils.FlattenRanges ( negatedRanges );
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

                    case NegatedCharacterTerminal negatedCharacterTerminal:
                        negatedCharacters.Add ( negatedCharacterTerminal.Value );
                        break;

                    case NegatedCharacterRange negatedCharacterRange:
                        negatedRanges.Add ( negatedCharacterRange.Range );
                        break;

                    case NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal:
                        negatedCategories.Add ( negatedUnicodeCategoryTerminal.Category );
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

            return new NegatedSet (
                set.Characters,
                set.NegatedCharacters,
                set.Ranges,
                set.NegatedRanges,
                set.UnicodeCategories,
                set.NegatedUnicodeCategories,
                set.Nodes,
                set.FlattenedRanges,
                set.NegatedFlattenedRanges,
                set.UnicodeCategoryFlagSet,
                set.NegatedUnicodeCategoryFlagSet );
        }
    }
}
