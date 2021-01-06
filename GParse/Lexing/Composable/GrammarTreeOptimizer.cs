using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Optimizes a grammar tree (might elide the entire tree if it's useless)
    /// </summary>
    public static class GrammarTreeOptimizer
    {
        private readonly struct OptimizeArgs
        {
            public readonly Boolean IsParentASet;

            public OptimizeArgs ( Boolean isParentASet )
            {
                this.IsParentASet = isParentASet;
            }

            [SuppressMessage ( "Performance", "CA1822:Mark members as static", Justification = "It might access instance data in the future." )]
            public OptimizeArgs WithIsParentASet ( Boolean isParentASet ) =>
                new ( isParentASet );
        }

        private sealed class Optimizer : GrammarTreeFolder<OptimizeArgs>
        {
            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitAlternation ( Alternation<Char> alternation, OptimizeArgs argument )
            {
                var nodes = alternation.GrammarNodes.Select ( node => this.Visit ( node, argument ) )
                                                    .Where ( node => node is not null )
                                                    .ToList ( );

                var matchedCharTerminals = new HashSet<Char> ( );
                var matchedStringTerminals = new HashSet<String> ( );
                var matchedCharRanges = new HashSet<Range<Char>> ( );
                var matchedUnicodeCategories = new HashSet<UnicodeCategory> ( );
                var matchedNodes = new HashSet<GrammarNode<Char>> ( GrammarTreeStructuralComparer.Instance );

                for ( var nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
                {
                loopStart:
                    GrammarNode<Char> currentNode = nodes[nodeIdx]!;

                    if ( matchedNodes.Contains ( currentNode ) )
                    {
                        nodes.RemoveAt ( nodeIdx );
                        goto loopStart;
                    }

                    switch ( currentNode )
                    {
                        case StringTerminal strTerminal:
                        {
                            if ( matchedStringTerminals.Contains ( strTerminal.Value ) )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                goto loopStart;
                            }
                            matchedStringTerminals.Add ( strTerminal.Value );
                            break;
                        }

                        case CharacterTerminal charTerminal:
                        {
                            var ch = charTerminal.Value;
                            if ( isCharMatched ( ch ) )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                goto loopStart;
                            }
                            matchedCharTerminals.Add ( charTerminal.Value );
                            break;
                        }

                        case CharacterRange characterRange:
                        {
                            var newStart = characterRange.Range.Start;
                            while ( isCharMatched ( newStart )
                                    && newStart <= characterRange.Range.End )
                            {
                                newStart++;
                            }

                            var newEnd = characterRange.Range.End;
                            while ( isCharMatched ( newEnd )
                                    && newEnd >= newStart )
                            {
                                newEnd--;
                            }

                            // The new start will be greater than the new end if all characters in the
                            // range are already being matched.
                            if ( newStart > newEnd )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                goto loopStart;
                            }
                            else if ( newStart == newEnd )
                            {
                                nodes[nodeIdx] = new CharacterTerminal ( newStart );
                                goto loopStart;
                            }
                            else if ( characterRange.Range.Start != newStart || characterRange.Range.End != newEnd )
                            {
                                nodes[nodeIdx] =
                                    currentNode =
                                    characterRange = new CharacterRange ( newStart, newEnd );
                            }
                            break;
                        }
                    }

                    if ( nodeIdx < nodes.Count - 1 )
                    {
                        GrammarNode<Char> nextNode = nodes[nodeIdx + 1]!;
                    }

                    matchedNodes.Add ( currentNode );
                }

                if ( nodes.Count == 0 )
                    return null;
                else if ( nodes.Count == 1 )
                    return nodes[0];
                else if ( nodes.SequenceEqual ( alternation.GrammarNodes ) )
                    return alternation;
                else
                    return new Alternation<Char> ( nodes! );

                Boolean isCharMatched ( Char ch ) =>
                    matchedCharTerminals!.Contains ( ch )
                    || matchedCharRanges!.Any ( range => CharUtils.IsInRange ( range.Start, ch, range.End ) )
                    || matchedUnicodeCategories!.Contains ( Char.GetUnicodeCategory ( ch ) );
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitSequence ( Sequence<Char> sequence, OptimizeArgs argument )
            {
                var nodes = sequence.GrammarNodes.Select ( node => this.Visit ( node, argument ) )
                                                 .Where ( node => node is not null )
                                                 .ToList ( );

                for ( var nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
                {
                loopStart:
                    GrammarNode<Char> currentNode = nodes[nodeIdx]!;

                    if ( nodeIdx < nodes.Count - 1 )
                    {
                        GrammarNode<Char> nextNode = nodes[nodeIdx + 1]!;
                        if ( currentNode is CharacterTerminal currentCharacterTerminal )
                        {
                            if ( nextNode is CharacterTerminal nextCharacterTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( new String ( new[] { currentCharacterTerminal.Value, nextCharacterTerminal.Value } ) );
                                goto loopStart;
                            }
                            else if ( nextNode is StringTerminal nextStringTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( currentCharacterTerminal.Value + nextStringTerminal.Value );
                                goto loopStart;
                            }
                        }
                        else if ( currentNode is StringTerminal currentStringTerminal )
                        {
                            if ( nextNode is CharacterTerminal nextCharacterTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( currentStringTerminal.Value + nextCharacterTerminal.Value );
                                goto loopStart;
                            }
                            else if ( nextNode is StringTerminal nextStringTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( currentStringTerminal.Value + nextStringTerminal.Value );
                                goto loopStart;
                            }
                        }
                    }
                }

                if ( nodes.Count == 0 )
                    return null;
                else if ( nodes.Count == 1 )
                    return nodes[0];
                else if ( nodes.SequenceEqual ( sequence.GrammarNodes ) )
                    return sequence;
                else
                    return new Sequence<Char> ( nodes! );
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitSet ( Set set, OptimizeArgs argument )
            {
                var characters = set.Characters.ToList ( );
                var negatedCharacters = new List<Char> ( );
                var ranges = set.Ranges.ToList ( );
                var negatedRanges = new List<Range<Char>> ( );
                var categories = set.UnicodeCategories.ToList ( );
                var negatedCategories = new List<UnicodeCategory> ( );
                // We don't want inner sets to be optimized since we'll flatten them.
                OptimizeArgs childrenArgument = argument.WithIsParentASet ( true );
                List<GrammarNode<Char>> nodes =
                    set.Nodes.Select ( node => this.Visit ( node, childrenArgument ) )
                             .Where ( node => node is not null )
                             .ToList ( )!;

                var nodeIdx = 0;
                while ( true )
                {
                loopStart:
                    if ( nodeIdx >= nodes.Count )
                        break;

                    GrammarNode<Char> node = nodes[nodeIdx]!;
                    switch ( node )
                    {
                        case CharacterTerminal characterTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            characters.Add ( characterTerminal.Value );
                            goto loopStart;

                        case NegatedCharacterTerminal negatedCharacterTerminal when !argument.IsParentASet:
                            nodes.RemoveAt ( nodeIdx );
                            negatedCharacters.Add ( negatedCharacterTerminal.Value );
                            goto loopStart;

                        case CharacterRange characterRange:
                            nodes.RemoveAt ( nodeIdx );
                            ranges.Add ( characterRange.Range );
                            goto loopStart;

                        case NegatedCharacterRange negatedCharacterRange when !argument.IsParentASet:
                            nodes.RemoveAt ( nodeIdx );
                            negatedRanges.Add ( negatedCharacterRange.Range );
                            goto loopStart;

                        case UnicodeCategoryTerminal unicodeCategoryTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            categories.Add ( unicodeCategoryTerminal.Category );
                            goto loopStart;

                        case NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal when !argument.IsParentASet:
                            nodes.RemoveAt ( nodeIdx );
                            negatedCategories.Add ( negatedUnicodeCategoryTerminal.Category );
                            goto loopStart;

                        case Set subSet:
                            nodes.RemoveAt ( nodeIdx );
                            characters.AddRange ( subSet.Characters );
                            ranges.AddRange ( subSet.Ranges );
                            categories.AddRange ( subSet.UnicodeCategories );
                            nodes.AddRange ( subSet.Nodes );
                            goto loopStart;
                    }
                    nodeIdx++;
                }

                // If the parent is a set, all we need to do is flattening,
                // so we don't optimize stuff as that might harm further
                // optimizations on the base set.
                if ( !argument.IsParentASet )
                {
                    characters.Sort ( );
                    OptimizationAlgorithms.ExpandRanges ( characters, ranges, true );
                    OptimizationAlgorithms.RangifyCharacters ( characters, ranges, true );
                    OptimizationAlgorithms.MergeRanges ( ranges );

                    negatedCharacters.Sort ( );
                    OptimizationAlgorithms.ExpandRanges ( negatedCharacters, negatedRanges, true );
                    OptimizationAlgorithms.RangifyCharacters ( negatedCharacters, negatedRanges, true );
                    OptimizationAlgorithms.MergeRanges ( negatedRanges );

                    ImmutableArray<Char> flattenedRanges = CharUtils.FlattenRanges ( ranges );
                    var categoriesFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
                    OptimizationAlgorithms.RemoveMatchedCharacters ( characters, flattenedRanges, categoriesFlagSet );

                    ImmutableArray<Char> negatedFlattenedRanges = CharUtils.FlattenRanges ( negatedRanges );
                    var negatedCategoriesFlagSet = CharUtils.CreateCategoryFlagSet ( negatedCategories );
                    OptimizationAlgorithms.RemoveMatchedCharacters ( negatedCharacters, negatedFlattenedRanges, negatedCategoriesFlagSet );

                    // Characters are still sorted at this point
                    CharacterBitVector? characterBitVector = null;
                    if ( characters.Any ( ) )
                    {
                        var charactersDistance = characters[characters.Count - 1] - characters[0];
                        if ( 1 < charactersDistance && charactersDistance <= 256 )
                            characterBitVector = new CharacterBitVector ( characters );
                    }

                    CharacterBitVector? negatedCharacterBitVector = null;
                    if ( negatedCharacters.Any ( ) )
                    {
                        var negatedCharactersDistance = negatedCharacters[negatedCharacters.Count - 1] - negatedCharacters[0];
                        if ( 1 < negatedCharactersDistance && negatedCharactersDistance <= 256 )
                            negatedCharacterBitVector = new CharacterBitVector ( negatedCharacters );
                    }

                    return new OptimizedSet (
                        characters.ToImmutableHashSet ( ),
                        negatedCharacters.ToImmutableHashSet ( ),
                        flattenedRanges,
                        negatedFlattenedRanges,
                        categoriesFlagSet,
                        negatedCategoriesFlagSet,
                        nodes.ToImmutableArray ( ),
                        characterBitVector,
                        negatedCharacterBitVector );
                }

                if ( !set.Characters.SetEquals ( characters )
                     || !set.Ranges.SetEquals ( ranges )
                     || set.UnicodeCategories.SetEquals ( categories )
                     || !set.Nodes.SetEquals ( nodes ) )
                {
                    return new Set (
                        characters.ToImmutableHashSet ( ),
                        ranges.ToImmutableHashSet ( ),
                        categories.ToImmutableHashSet ( ),
                        nodes.ToImmutableHashSet ( ) );
                }
                return set;
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitNegatedSet ( NegatedSet negatedSet, OptimizeArgs argument )
            {
                var characters = negatedSet.Characters.ToList ( );
                var negatedCharacters = new List<Char> ( );
                var ranges = negatedSet.Ranges.ToList ( );
                var negatedRanges = new List<Range<Char>> ( );
                var categories = negatedSet.UnicodeCategories.ToList ( );
                var negatedCategories = new List<UnicodeCategory> ( );
                // We don't want inner sets to be optimized since we'll flatten them.
                OptimizeArgs childrenArgument = argument.WithIsParentASet ( true );
                List<GrammarNode<Char>> nodes =
                    negatedSet.Nodes.Select ( node => this.Visit ( node, childrenArgument ) )
                             .Where ( node => node is not null )
                             .ToList ( )!;

                var nodeIdx = 0;
                while ( true )
                {
                loopStart:
                    if ( nodeIdx >= nodes.Count )
                        break;

                    GrammarNode<Char> node = nodes[nodeIdx]!;
                    switch ( node )
                    {
                        case CharacterTerminal characterTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            characters.Add ( characterTerminal.Value );
                            goto loopStart;

                        case NegatedCharacterTerminal negatedCharacterTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            negatedCharacters.Add ( negatedCharacterTerminal.Value );
                            goto loopStart;

                        case CharacterRange characterRange:
                            nodes.RemoveAt ( nodeIdx );
                            ranges.Add ( characterRange.Range );
                            goto loopStart;

                        case NegatedCharacterRange negatedCharacterRange:
                            nodes.RemoveAt ( nodeIdx );
                            negatedRanges.Add ( negatedCharacterRange.Range );
                            goto loopStart;

                        case UnicodeCategoryTerminal unicodeCategoryTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            categories.Add ( unicodeCategoryTerminal.Category );
                            goto loopStart;

                        case NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal:
                            nodes.RemoveAt ( nodeIdx );
                            negatedCategories.Add ( negatedUnicodeCategoryTerminal.Category );
                            goto loopStart;

                        case Set subSet:
                            nodes.RemoveAt ( nodeIdx );
                            characters.AddRange ( subSet.Characters );
                            ranges.AddRange ( subSet.Ranges );
                            categories.AddRange ( subSet.UnicodeCategories );
                            nodes.AddRange ( subSet.Nodes );
                            goto loopStart;
                    }
                    nodeIdx++;
                }

                characters.Sort ( );
                OptimizationAlgorithms.ExpandRanges ( characters, ranges, true );
                OptimizationAlgorithms.RangifyCharacters ( characters, ranges, true );
                OptimizationAlgorithms.MergeRanges ( ranges );

                negatedCharacters.Sort ( );
                OptimizationAlgorithms.ExpandRanges ( negatedCharacters, negatedRanges, true );
                OptimizationAlgorithms.RangifyCharacters ( negatedCharacters, negatedRanges, true );
                OptimizationAlgorithms.MergeRanges ( negatedRanges );

                ImmutableArray<Char> flattenedRanges = CharUtils.FlattenRanges ( ranges );
                var categoriesFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
                OptimizationAlgorithms.RemoveMatchedCharacters ( characters, flattenedRanges, categoriesFlagSet );

                ImmutableArray<Char> negatedFlattenedRanges = CharUtils.FlattenRanges ( negatedRanges );
                var negatedCategoriesFlagSet = CharUtils.CreateCategoryFlagSet ( negatedCategories );
                OptimizationAlgorithms.RemoveMatchedCharacters ( negatedCharacters, negatedFlattenedRanges, negatedCategoriesFlagSet );

                // Characters are still sorted at this point
                CharacterBitVector? characterBitVector = null;
                if ( characters.Any ( ) )
                {
                    var charactersDistance = characters[characters.Count - 1] - characters[0];
                    if ( 1 < charactersDistance && charactersDistance <= 256 )
                        characterBitVector = new CharacterBitVector ( characters );
                }

                CharacterBitVector? negatedCharacterBitVector = null;
                if ( negatedCharacters.Any ( ) )
                {
                    var negatedCharactersDistance = negatedCharacters[negatedCharacters.Count - 1] - negatedCharacters[0];
                    if ( 1 < negatedCharactersDistance && negatedCharactersDistance <= 256 )
                        negatedCharacterBitVector = new CharacterBitVector ( negatedCharacters );
                }

                return new OptimizedNegatedSet (
                    characters.ToImmutableHashSet ( ),
                    negatedCharacters.ToImmutableHashSet ( ),
                    flattenedRanges,
                    negatedFlattenedRanges,
                    categoriesFlagSet,
                    negatedCategoriesFlagSet,
                    nodes.ToImmutableArray ( ),
                    characterBitVector,
                    negatedCharacterBitVector );
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitStringTerminal ( StringTerminal stringTerminal, OptimizeArgs argument )
            {
                if ( stringTerminal.Value.Length == 1 )
                    return new CharacterTerminal ( stringTerminal.Value[0] );
                else
                    return stringTerminal;
            }
        }

        private static readonly Optimizer optimizer = new ( );

        /// <summary>
        /// Optimizes the provided tree.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static GrammarNode<Char>? Optimize ( this GrammarNode<Char> node ) =>
            optimizer.Visit ( node, default );
    }
}