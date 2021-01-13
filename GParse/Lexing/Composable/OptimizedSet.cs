using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An optimized form of <see cref="Set"/>.
    /// Currently can only be created by the <see cref="GrammarTreeOptimizer"/>.
    /// </summary>
    public sealed class OptimizedSet : GrammarNode<Char>, IEquatable<OptimizedSet?>
    {
        /// <summary>
        /// A bitvector of characters matched by this set..
        /// Available when there are few characters and they're close enough.
        /// </summary>
        public CharacterBitVector? CharaterBitVector { get; }

        /// <summary>
        /// The characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The (flattened) character ranges matched by this set.
        /// </summary>
        public ImmutableArray<Char> FlattenedRanges { get; }

        /// <summary>
        /// The flagset of the unicode categories matched by this set.
        /// </summary>
        public UInt32 UnicodeCategoryFlagSet { get; }

        /// <summary>
        /// The nodes that the set matches.
        /// </summary>
        public ImmutableArray<GrammarNode<Char>> Nodes { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterOptimizedSet;

        /// <summary>
        /// Initializes a new optimized set.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="flattenedRanges"></param>
        /// <param name="unicodeCategoryFlagSet"></param>
        /// <param name="nodes"></param>
        /// <param name="characterBitVector"></param>
        internal OptimizedSet (
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

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as OptimizedSet );

        /// <inheritdoc/>
        public Boolean Equals ( OptimizedSet? other ) =>
            other != null
            && this.UnicodeCategoryFlagSet == other.UnicodeCategoryFlagSet
            && EqualityComparer<CharacterBitVector?>.Default.Equals ( this.CharaterBitVector, other.CharaterBitVector )
            && this.Characters.SetEquals ( other.Characters )
            && this.FlattenedRanges.SequenceEqual ( other.FlattenedRanges )
            && this.Nodes.SequenceEqual ( other.Nodes );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( )
        {
            var hash = new HashCode ( );
            hash.Add ( this.CharaterBitVector );
            foreach ( var ch in this.Characters ) hash.Add ( ch );
            foreach ( var elem in this.FlattenedRanges ) hash.Add ( elem );
            hash.Add ( this.UnicodeCategoryFlagSet );
            foreach ( GrammarNode<Char> node in this.Nodes ) hash.Add ( node );
            return hash.ToHashCode ( );
        }

        /// <summary>
        /// Negated an optimized set.
        /// </summary>
        /// <param name="optimizedSet"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's the Negate extension method." )]
        public static OptimizedNegatedSet operator ! ( OptimizedSet optimizedSet )
        {
            if ( optimizedSet is null )
                throw new ArgumentNullException ( nameof ( optimizedSet ) );
            return new OptimizedNegatedSet (
                optimizedSet.Characters,
                optimizedSet.FlattenedRanges,
                optimizedSet.UnicodeCategoryFlagSet,
                optimizedSet.Nodes,
                optimizedSet.CharaterBitVector );
        }

        /// <summary>
        /// Checks whether two optimized sets are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( OptimizedSet? left, OptimizedSet? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two optimized sets are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( OptimizedSet? left, OptimizedSet? right ) =>
            !( left == right );
    }
}
