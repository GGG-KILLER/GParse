using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A structural comparer for grammar trees.
    /// </summary>
    public class GrammarTreeStructuralComparer : IEqualityComparer<GrammarNode<Char>>
    {
        private class EqualityComparer : GrammarTreeVisitor<Boolean, GrammarNode<Char>>
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
                && set.Ranges.SequenceEqual ( set2.Ranges )
                && set.UnicodeCategoryFlagSet == set2.UnicodeCategoryFlagSet;

            protected override Boolean VisitCharacterTerminal ( CharacterTerminal characterTerminal, GrammarNode<Char> argument ) =>
                argument is CharacterTerminal characterTerminal2 && characterTerminal.Value == characterTerminal2.Value;

            protected override Boolean VisitLookahead ( Lookahead lookahead, GrammarNode<Char> argument ) =>
                argument is Lookahead lookahead2
                && this.Visit ( lookahead.InnerNode, lookahead2.InnerNode );

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
                && negatedSet.Ranges.Equals ( negatedSet2.Ranges )
                && negatedSet.UnicodeCategoryFlagSet == negatedSet2.UnicodeCategoryFlagSet;

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
                && numberedCapture.Position == numberedCapture2.Position && this.Visit ( numberedCapture, numberedCapture2 );

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
                && stringTerminal.String.Equals ( stringTerminal2.String, StringComparison.Ordinal );

            protected override Boolean VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, GrammarNode<Char> argument ) =>
                argument is UnicodeCategoryTerminal unicodeCategoryTerminal2
                && unicodeCategoryTerminal.Category == unicodeCategoryTerminal2.Category;

            public override Boolean Visit ( GrammarNode<Char> grammarNode, GrammarNode<Char> argument ) =>
                ReferenceEquals ( grammarNode, argument )
                || ( grammarNode is not null && argument is not null && base.Visit ( grammarNode, argument ) );
        }
        private class Hasher : GrammarTreeVisitor<Int32, Unit>
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
                foreach ( var character in set.Characters.OrderBy ( character => character ) )
                    hash.Add ( character );
                foreach ( Range<Char> range in set.Ranges )
                    hash.Add ( range );
                hash.Add ( set.UnicodeCategoryFlagSet );
                foreach ( GrammarNode<Char>? node in set.Nodes )
                    hash.Add ( this.Visit ( node, default ) );
                return hash.ToHashCode ( );
            }

            protected override Int32 VisitCharacterTerminal ( CharacterTerminal characterTerminal, Unit argument ) =>
                HashCode.Combine ( characterTerminal.Value );

            protected override Int32 VisitLookahead ( Lookahead lookahead, Unit argument ) =>
                this.Visit ( lookahead.InnerNode, default );

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
                foreach ( var character in negatedSet.Characters.OrderBy ( character => character ) )
                    hash.Add ( character );
                foreach ( Range<Char> range in negatedSet.Ranges )
                    hash.Add ( range );
                hash.Add ( negatedSet.UnicodeCategoryFlagSet );
                foreach ( GrammarNode<Char> node in negatedSet.Nodes )
                    hash.Add ( this.Visit ( node, default ) );
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
                HashCode.Combine ( characterTerminalString.String );

            protected override Int32 VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, Unit argument ) =>
                HashCode.Combine ( unicodeCategoryTerminal.Category );

            public override Int32 Visit ( GrammarNode<Char> grammarNode, Unit argument )
            {
                if ( grammarNode is null )
                    return 0;
                return base.Visit ( grammarNode, argument );
            }
        }

        /// <summary>
        /// The instance of the comparer.
        /// </summary>
        public static GrammarTreeStructuralComparer Instance { get; } = new GrammarTreeStructuralComparer ( );
        private readonly EqualityComparer equalityComparer;
        private readonly Hasher hasher;

        /// <summary>
        /// Initializes a new grammar tree comparer.
        /// </summary>
        public GrammarTreeStructuralComparer ( )
        {
            this.equalityComparer = new EqualityComparer ( this );
            this.hasher = new Hasher ( this );
        }

        /// <inheritdoc/>
        public Boolean Equals ( GrammarNode<Char>? x, GrammarNode<Char>? y ) =>
            ReferenceEquals ( x, y )
            || ( x is not null && y is not null && this.equalityComparer.Visit ( x, y ) );

        /// <inheritdoc/>
        public Int32 GetHashCode ( GrammarNode<Char> obj ) => this.hasher.Visit ( obj, default );
    }
}
