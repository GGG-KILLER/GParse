using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        /// The ranges not matched by this set.
        /// </summary>
        public IImmutableSet<Range<Char>> Ranges { get; }

        /// <summary>
        /// The unicode categories not matched by this set.
        /// </summary>
        public IImmutableSet<UnicodeCategory> UnicodeCategories { get; }

        /// <summary>
        /// The nodes not matched by this set.
        /// </summary>
        public IImmutableSet<GrammarNode<Char>> Nodes { get; }

        /// <summary>
        /// Initializes a new negated set from its elements.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategories"></param>
        /// <param name="nodes"></param>
        public NegatedSet (
            IImmutableSet<Char> characters,
            IImmutableSet<Range<Char>> ranges,
            IImmutableSet<UnicodeCategory> unicodeCategories,
            IImmutableSet<GrammarNode<Char>> nodes )
        {
            this.Characters = characters ?? throw new ArgumentNullException ( nameof ( characters ) );
            this.Ranges = ranges;
            this.UnicodeCategories = unicodeCategories;
            this.Nodes = nodes;
        }

        /// <summary>
        /// Initializes a new alternation set.
        /// </summary>
        /// <param name="setElements">The elements of this set.</param>
        public NegatedSet ( params SetElement[] setElements )
        {
            if ( setElements is null )
                throw new ArgumentNullException ( nameof ( setElements ) );
            if ( setElements.Length < 1 )
                throw new ArgumentException ( "At least 1 set element is required.", nameof ( setElements ) );

            ImmutableHashSet<Char>.Builder characters = ImmutableHashSet.CreateBuilder<Char> ( );
            ImmutableHashSet<Range<Char>>.Builder ranges = ImmutableHashSet.CreateBuilder<Range<Char>> ( );
            ImmutableHashSet<UnicodeCategory>.Builder categories = ImmutableHashSet.CreateBuilder<UnicodeCategory> ( );
            ImmutableHashSet<GrammarNode<Char>>.Builder nodes = ImmutableHashSet.CreateBuilder<GrammarNode<Char>> ( );
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
                        nodes.Add ( setElement.Node );
                        break;
                    case SetElementType.Invalid:
                    default:
                        throw new InvalidOperationException ( $"Invalid set element provided at index {elementIdx}." );
                }
            }

            this.Characters = characters.ToImmutable ( );
            this.Ranges = ranges.ToImmutable ( );
            this.UnicodeCategories = categories.ToImmutable ( );
            this.Nodes = nodes.ToImmutable ( );
        }


        /// <summary>
        /// Un-negates this set.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method exists." )]
        public static Set operator ! ( NegatedSet negatedSet )
        {
            if ( negatedSet is null )
                throw new ArgumentNullException ( nameof ( negatedSet ) );

            return new Set (
                negatedSet.Characters,
                negatedSet.Ranges,
                negatedSet.UnicodeCategories,
                negatedSet.Nodes );
        }
    }
}
