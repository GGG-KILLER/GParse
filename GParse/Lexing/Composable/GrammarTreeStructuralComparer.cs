using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A structural comparer for grammar trees.
    /// </summary>
    public sealed class GrammarTreeStructuralComparer : IEqualityComparer<GrammarNode<Char>>
    {
        private sealed class EqualityComparer : GrammarTreeVisitor<Boolean, GrammarNode<Char>>
        {
            private readonly GrammarTreeStructuralComparer _treeComparer;

            public EqualityComparer ( GrammarTreeStructuralComparer treeComparer )
            {
                this._treeComparer = treeComparer ?? throw new ArgumentNullException ( nameof ( treeComparer ) );
            }

            protected override Boolean VisitAlternation ( Alternation<Char> leftAlternation, GrammarNode<Char> argument ) =>
                 argument is Alternation<Char> rightAlternation
                 && leftAlternation.GrammarNodes.SequenceEqual ( rightAlternation.GrammarNodes, this._treeComparer );

            protected override Boolean VisitAny ( Any any, GrammarNode<Char> argument ) =>
                argument is Any;

            protected override Boolean VisitCharacterRange ( CharacterRange characterRange, GrammarNode<Char> argument ) =>
                argument is CharacterRange characterRange2
                && characterRange.Range == characterRange2.Range;

            protected override Boolean VisitSet ( Set set, GrammarNode<Char> argument ) =>
                argument is Set set2
                && set.Characters.SetEquals ( set2.Characters )
                && set.Ranges.SetEquals ( set2.Ranges )
                && set.UnicodeCategories.SetEquals ( set2.UnicodeCategories )
                && set.Nodes.SetEquals ( set2.Nodes );

            protected override Boolean VisitCharacterTerminal ( CharacterTerminal characterTerminal, GrammarNode<Char> argument ) =>
                argument is CharacterTerminal characterTerminal2 && characterTerminal.Value == characterTerminal2.Value;

            protected override Boolean VisitPositiveLookahead ( PositiveLookahead positiveLookahead, GrammarNode<Char> argument ) =>
                argument is PositiveLookahead positiveLookahead2
                && this.Visit ( positiveLookahead.InnerNode, positiveLookahead2.InnerNode );

            protected override Boolean VisitNamedBackreference ( NamedBackreference namedBackreference, GrammarNode<Char> argument ) =>
                argument is NamedBackreference namedBackreference2
                && namedBackreference.Name.Equals ( namedBackreference2.Name, StringComparison.Ordinal );

            protected override Boolean VisitNamedCapture ( NamedCapture namedCapture, GrammarNode<Char> argument ) =>
                argument is NamedCapture namedCapture2
                && namedCapture.Name.Equals ( namedCapture2.Name )
                && this.Visit ( namedCapture.InnerNode, namedCapture2.InnerNode );

            protected override Boolean VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, GrammarNode<Char> argument ) =>
                argument is NegatedCharacterRange negatedCharacterRange2
                && negatedCharacterRange.Range == negatedCharacterRange2.Range;

            protected override Boolean VisitNegatedSet ( NegatedSet negatedSet, GrammarNode<Char> argument ) =>
                argument is NegatedSet negatedSet2
                && negatedSet.Characters.SetEquals ( negatedSet2.Characters )
                && negatedSet.Ranges.SetEquals ( negatedSet2.Ranges )
                && negatedSet.UnicodeCategories.SetEquals ( negatedSet2.UnicodeCategories )
                && negatedSet.Nodes.SetEquals ( negatedSet2.Nodes );

            protected override Boolean VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, GrammarNode<Char> argument ) =>
                argument is NegatedCharacterTerminal negatedCharacterTerminal2 && negatedCharacterTerminal.Value == negatedCharacterTerminal2.Value;

            protected override Boolean VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, GrammarNode<Char> argument ) =>
                argument is NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal2
                && negatedUnicodeCategoryTerminal.Category == negatedUnicodeCategoryTerminal2.Category;

            protected override Boolean VisitNegativeLookahead ( NegativeLookahead negativeLookahead, GrammarNode<Char> argument ) =>
                argument is NegativeLookahead negativeLookahead2
                && this.Visit ( negativeLookahead.InnerNode, negativeLookahead2.InnerNode );

            protected override Boolean VisitNumberedBackreference ( NumberedBackreference numberedBackreference, GrammarNode<Char> argument ) =>
                argument is NumberedBackreference numberedBackreference2
                && numberedBackreference.Position == numberedBackreference2.Position;

            protected override Boolean VisitNumberedCapture ( NumberedCapture numberedCapture, GrammarNode<Char> argument ) =>
                argument is NumberedCapture numberedCapture2
                && numberedCapture.Position == numberedCapture2.Position
                && this.Visit ( numberedCapture.InnerNode, numberedCapture2.InnerNode );

            protected override Boolean VisitRepetition ( Repetition<Char> repetition, GrammarNode<Char> argument ) =>
                argument is Repetition<Char> repetition2
                && repetition.IsLazy == repetition2.IsLazy
                && repetition.Range == repetition2.Range
                && this.Visit ( repetition.InnerNode, repetition2.InnerNode );

            protected override Boolean VisitSequence ( Sequence<Char> sequence, GrammarNode<Char> argument ) =>
                argument is Sequence<Char> sequence2
                && sequence.GrammarNodes.SequenceEqual ( sequence2.GrammarNodes, this._treeComparer );

            protected override Boolean VisitStringTerminal ( StringTerminal stringTerminal, GrammarNode<Char> argument ) =>
                argument is StringTerminal stringTerminal2
                && stringTerminal.Value.Equals ( stringTerminal2.Value, StringComparison.Ordinal );

            protected override Boolean VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, GrammarNode<Char> argument ) =>
                argument is UnicodeCategoryTerminal unicodeCategoryTerminal2
                && unicodeCategoryTerminal.Category == unicodeCategoryTerminal2.Category;

            protected override Boolean VisitOptimizedSet ( OptimizedSet optimizedSet1, GrammarNode<Char> argument ) =>
                argument is OptimizedSet optimizedSet2
                && optimizedSet1.Characters.SetEquals ( optimizedSet2.Characters )
                && optimizedSet1.NegatedCharacters.SetEquals ( optimizedSet2.NegatedCharacters )
                && optimizedSet1.FlattenedRanges.SequenceEqual ( optimizedSet2.FlattenedRanges )
                && optimizedSet1.NegatedFlattenedRanges.SequenceEqual ( optimizedSet2.NegatedFlattenedRanges )
                && optimizedSet1.UnicodeCategoryFlagSet == optimizedSet2.UnicodeCategoryFlagSet
                && optimizedSet1.NegatedUnicodeCategoryFlagSet == optimizedSet2.NegatedUnicodeCategoryFlagSet
                && optimizedSet1.Nodes.SequenceEqual ( optimizedSet2.Nodes, this._treeComparer );

            protected override Boolean VisitOptimizedNegatedSet ( OptimizedNegatedSet optimizedNegatedSet1, GrammarNode<Char> argument ) =>
                argument is OptimizedNegatedSet optimizedNegatedSet2
                && optimizedNegatedSet1.Characters.SetEquals ( optimizedNegatedSet2.Characters )
                && optimizedNegatedSet1.NegatedCharacters.SetEquals ( optimizedNegatedSet2.NegatedCharacters )
                && optimizedNegatedSet1.FlattenedRanges.SequenceEqual ( optimizedNegatedSet2.FlattenedRanges )
                && optimizedNegatedSet1.NegatedFlattenedRanges.SequenceEqual ( optimizedNegatedSet2.NegatedFlattenedRanges )
                && optimizedNegatedSet1.UnicodeCategoryFlagSet == optimizedNegatedSet2.UnicodeCategoryFlagSet
                && optimizedNegatedSet1.NegatedUnicodeCategoryFlagSet == optimizedNegatedSet2.NegatedUnicodeCategoryFlagSet
                && optimizedNegatedSet1.Nodes.SequenceEqual ( optimizedNegatedSet2.Nodes, this._treeComparer );
        }

        private sealed class Hasher : GrammarTreeVisitor<Int32, Unit>
        {
            private readonly GrammarTreeStructuralComparer treeComparer;

            public Hasher ( GrammarTreeStructuralComparer treeComparer )
            {
                this.treeComparer = treeComparer ?? throw new ArgumentNullException ( nameof ( treeComparer ) );
            }

            protected override Int32 VisitAlternation ( Alternation<Char> alternation, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( GrammarNode<Char> elem in alternation.GrammarNodes )
                    hash.Add ( elem, this.treeComparer );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitAny ( Any any, Unit argument ) =>
                0;

            protected override Int32 VisitCharacterRange ( CharacterRange characterRange, Unit argument ) =>
                HashCode.Combine ( characterRange.Range );

            protected override Int32 VisitSet ( Set set, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( var character in set.Characters ) hash.Add ( character );
                foreach ( Range<Char> range in set.Ranges ) hash.Add ( range );
                foreach ( UnicodeCategory category in set.UnicodeCategories ) hash.Add ( category );
                foreach ( GrammarNode<Char> node in set.Nodes ) hash.Add ( this.Visit ( node, argument ) );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitCharacterTerminal ( CharacterTerminal characterTerminal, Unit argument ) =>
                HashCode.Combine ( characterTerminal.Value );

            protected override Int32 VisitPositiveLookahead ( PositiveLookahead positiveLookahead, Unit argument ) =>
                this.Visit ( positiveLookahead.InnerNode, default );

            protected override Int32 VisitNamedBackreference ( NamedBackreference namedBackreference, Unit argument ) =>
                HashCode.Combine ( namedBackreference.Name );

            protected override Int32 VisitNamedCapture ( NamedCapture namedCapture, Unit argument )
            {
                var hash = new HashCode ( );
                hash.Add ( namedCapture.Name );
                hash.Add ( namedCapture.InnerNode, this.treeComparer );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, Unit argument ) =>
                HashCode.Combine ( negatedCharacterRange.Range );

            protected override Int32 VisitNegatedSet ( NegatedSet negatedSet, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( var character in negatedSet.Characters ) hash.Add ( character );
                foreach ( Range<Char> range in negatedSet.Ranges ) hash.Add ( range );
                foreach ( UnicodeCategory category in negatedSet.UnicodeCategories ) hash.Add ( category );
                foreach ( GrammarNode<Char> node in negatedSet.Nodes ) hash.Add ( this.Visit ( node, argument ) );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, Unit argument ) =>
                HashCode.Combine ( negatedCharacterTerminal.Value );

            protected override Int32 VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, Unit argument ) =>
                HashCode.Combine ( negatedUnicodeCategoryTerminal.Category );

            protected override Int32 VisitNegativeLookahead ( NegativeLookahead negativeLookahead, Unit argument ) =>
                this.Visit ( negativeLookahead.InnerNode, default );

            protected override Int32 VisitNumberedBackreference ( NumberedBackreference numberedBackreference, Unit argument ) =>
               HashCode.Combine ( numberedBackreference.Position );

            protected override Int32 VisitNumberedCapture ( NumberedCapture numberedCapture, Unit argument )
            {
                var hash = new HashCode ( );
                hash.Add ( numberedCapture.Position );
                hash.Add ( numberedCapture.InnerNode, this.treeComparer );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitRepetition ( Repetition<Char> repetition, Unit argument )
            {
                var hash = new HashCode ( );
                hash.Add ( repetition.IsLazy );
                hash.Add ( repetition.Range.Minimum );
                hash.Add ( repetition.Range.Maximum );
                hash.Add ( repetition.InnerNode, this.treeComparer );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitSequence ( Sequence<Char> sequence, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( GrammarNode<Char> node in sequence.GrammarNodes )
                    hash.Add ( node, this.treeComparer );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitStringTerminal ( StringTerminal characterTerminalString, Unit argument ) =>
                HashCode.Combine ( characterTerminalString.Value );

            protected override Int32 VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, Unit argument ) =>
                HashCode.Combine ( unicodeCategoryTerminal.Category );

            public override Int32 Visit ( GrammarNode<Char> grammarNode, Unit argument )
            {
                if ( grammarNode is null )
                    return 0;
                return base.Visit ( grammarNode, argument );
            }

            protected override Int32 VisitOptimizedSet ( OptimizedSet optimizedSet, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( var character in optimizedSet.Characters ) hash.Add ( character );
                foreach ( var negatedCharacter in optimizedSet.NegatedCharacters ) hash.Add ( negatedCharacter );
                foreach ( var rangeElement in optimizedSet.FlattenedRanges ) hash.Add ( rangeElement );
                foreach ( var negatedRangeElement in optimizedSet.NegatedFlattenedRanges ) hash.Add ( negatedRangeElement );
                hash.Add ( optimizedSet.UnicodeCategoryFlagSet );
                hash.Add ( optimizedSet.NegatedUnicodeCategoryFlagSet );
                foreach ( GrammarNode<Char> node in optimizedSet.Nodes ) hash.Add ( this.Visit ( node, argument ) );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitOptimizedNegatedSet ( OptimizedNegatedSet optimizedNegatedSet, Unit argument )
            {
                var hash = new HashCode ( );
                foreach ( var character in optimizedNegatedSet.Characters ) hash.Add ( character );
                foreach ( var negatedCharacter in optimizedNegatedSet.NegatedCharacters ) hash.Add ( negatedCharacter );
                foreach ( var rangeElement in optimizedNegatedSet.FlattenedRanges ) hash.Add ( rangeElement );
                foreach ( var negatedRangeElement in optimizedNegatedSet.NegatedFlattenedRanges ) hash.Add ( negatedRangeElement );
                hash.Add ( optimizedNegatedSet.UnicodeCategoryFlagSet );
                hash.Add ( optimizedNegatedSet.NegatedUnicodeCategoryFlagSet );
                foreach ( GrammarNode<Char> node in optimizedNegatedSet.Nodes ) hash.Add ( this.Visit ( node, argument ) );
                return hash.ToHashCode ( );
            }
        }

        /// <summary>
        /// The instance of the comparer.
        /// </summary>
        public static GrammarTreeStructuralComparer Instance { get; } = new GrammarTreeStructuralComparer ( );
        private readonly EqualityComparer _equalityComparer;
        private readonly Hasher _hasher;

        /// <summary>
        /// Initializes a new grammar tree comparer.
        /// </summary>
        public GrammarTreeStructuralComparer ( )
        {
            this._equalityComparer = new EqualityComparer ( this );
            this._hasher = new Hasher ( this );
        }

        /// <inheritdoc/>
        public Boolean Equals ( GrammarNode<Char>? x, GrammarNode<Char>? y ) =>
            ReferenceEquals ( x, y )
            || ( x is not null && y is not null && this._equalityComparer.Visit ( x, y ) );

        /// <inheritdoc/>
        public Int32 GetHashCode ( GrammarNode<Char> obj ) =>
            this._hasher.Visit ( obj, default );
    }
}
