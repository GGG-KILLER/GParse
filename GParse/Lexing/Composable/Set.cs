using System;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
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
        /// The (flattened) character ranges matched by this set.
        /// </summary>
        public ImmutableArray<Char> Ranges { get; }

        /// <summary>
        /// The flagset of the unicode categories matched by this set.
        /// </summary>
        public UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// Initializes a new set from its elements.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategoryBitSet"></param>
        internal Set ( IImmutableSet<Char> characters, ImmutableArray<Char> ranges, UInt32 unicodeCategoryBitSet )
        {
            this.Characters = characters;
            this.Ranges = ranges;
            this.UnicodeCategoryFlagSet = unicodeCategoryBitSet;
        }

        /// <summary>
        /// Initializes a new alternation set.
        /// </summary>
        /// <param name="setElements">The elements of this set.</param>
        public Set ( params SetElement[] setElements )
        {
            this.Characters = setElements.Where ( elem => elem.Type == SetElementType.Character )
                                         .Select ( elem => elem.Character )
                                         .ToImmutableHashSet ( );
            this.UnicodeCategoryFlagSet = CharUtils.CreateCategoryFlagSet (
                setElements.Where ( elem => elem.Type == SetElementType.UnicodeCategory )
                           .Select ( elem => elem.UnicodeCategory ) );
            this.Ranges = CharUtils.FlattenRanges ( setElements.Where ( elem => elem.Type == SetElementType.Range )
                                                               .Select ( elem => elem.Range ) );
        }

        /// <summary>
        /// Negates this set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static NegatedSet operator ! ( Set set ) =>
            new NegatedSet ( set.Characters, set.Ranges, set.UnicodeCategoryFlagSet );
    }
}
