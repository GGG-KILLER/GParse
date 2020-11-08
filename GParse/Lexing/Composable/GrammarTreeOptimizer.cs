using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
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
        private sealed class Optimizer : GrammarTreeFolder<Unit>
        {
            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitAlternation ( Alternation<Char> alternation, Unit argument )
            {
                var nodes = alternation.GrammarNodes.Select ( node => this.Visit ( node, default ) )
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
                            if ( matchedStringTerminals.Contains ( strTerminal.String ) )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                goto loopStart;
                            }
                            matchedStringTerminals.Add ( strTerminal.String );
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
            protected override GrammarNode<Char>? VisitSequence ( Sequence<Char> sequence, Unit argument )
            {
                var nodes = sequence.GrammarNodes.Select ( node => this.Visit ( node, Unit.Value ) )
                                                 .Where ( node => node is not null )
                                                 .ToList ( );
                var builder = new StringBuilder ( );

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
                                nodes[nodeIdx] = new StringTerminal ( currentCharacterTerminal.Value + nextStringTerminal.String );
                                goto loopStart;
                            }
                        }
                        else if ( currentNode is StringTerminal currentStringTerminal )
                        {
                            if ( nextNode is CharacterTerminal nextCharacterTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( currentStringTerminal.String + nextCharacterTerminal.Value );
                                goto loopStart;
                            }
                            else if ( nextNode is StringTerminal nextStringTerminal )
                            {
                                nodes.RemoveAt ( nodeIdx );
                                nodes[nodeIdx] = new StringTerminal ( currentStringTerminal.String + nextStringTerminal.String );
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
            protected override GrammarNode<Char>? VisitSet ( Set set, Unit argument )
            {
                var characters = set.Characters.ToList ( );
                var ranges = set.Ranges.ToList ( );
                var categories = set.UnicodeCategories.ToList ( );
                var nodes = set.Nodes.Select ( node => this.Visit ( node, default ) )
                                     .Where ( node => node is not null )
                                     .ToList ( );

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

                        case CharacterRange characterRange:
                            nodes.RemoveAt ( nodeIdx );
                            ranges.Add ( characterRange.Range );
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

                categories.Sort ( );
                OptimizationAlgorithms.ExpandRanges ( characters, ranges, true );
                OptimizationAlgorithms.RangifyCharacters ( characters, ranges, true );
                OptimizationAlgorithms.MergeRanges ( ranges );

                ImmutableArray<Char> flattenedRanges = CharUtils.FlattenRanges ( ranges );
                var categoriesFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
                OptimizationAlgorithms.RemoveMatchedCharacters ( characters, flattenedRanges, categoriesFlagSet );

                var newSet = new Set ( characters.ToImmutableHashSet ( ), ranges.ToImmutableArray ( ), categories.ToImmutableArray ( ), nodes.ToImmutableArray ( )!, flattenedRanges, categoriesFlagSet );

                if ( !set.Characters.SetEquals ( newSet.Characters )
                     || !set.Ranges.SequenceEqual ( newSet.Ranges )
                     || set.UnicodeCategoryFlagSet != newSet.UnicodeCategoryFlagSet
                     || !set.Nodes.SequenceEqual ( newSet.Nodes ) )
                {
                    return newSet;
                }

                return set;
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitNegatedSet ( NegatedSet negatedSet, Unit argument )
            {
                var characters = negatedSet.Characters.ToList ( );
                var ranges = negatedSet.Ranges.ToList ( );
                var categories = negatedSet.UnicodeCategories.ToList ( );
                var nodes = negatedSet.Nodes.Select ( node => this.Visit ( node, default ) )
                                            .Where ( node => node is not null )
                                            .ToList ( );

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

                        case CharacterRange characterRange:
                            nodes.RemoveAt ( nodeIdx );
                            ranges.Add ( characterRange.Range );
                            goto loopStart;

                        case NegatedSet subNegatedSet:
                            nodes.RemoveAt ( nodeIdx );
                            characters.AddRange ( subNegatedSet.Characters );
                            ranges.AddRange ( subNegatedSet.Ranges );
                            categories.AddRange ( subNegatedSet.UnicodeCategories );
                            nodes.AddRange ( subNegatedSet.Nodes );
                            goto loopStart;
                    }
                    nodeIdx++;
                }

                characters.Sort ( );
                OptimizationAlgorithms.ExpandRanges ( characters, ranges, true );
                OptimizationAlgorithms.RangifyCharacters ( characters, ranges, true );
                OptimizationAlgorithms.MergeRanges ( ranges );

                ImmutableArray<Char> flattenedRanges = CharUtils.FlattenRanges ( ranges );
                var categoriesFlagSet = CharUtils.CreateCategoryFlagSet ( categories );
                OptimizationAlgorithms.RemoveMatchedCharacters ( characters, flattenedRanges, categoriesFlagSet );

                var newNegatedSet = new NegatedSet ( characters.ToImmutableHashSet ( ), ranges.ToImmutableArray ( ), categories.ToImmutableArray ( ), nodes.ToImmutableArray ( )!, flattenedRanges, categoriesFlagSet );
                if ( !negatedSet.Characters.SetEquals ( newNegatedSet.Characters )
                     || !negatedSet.Ranges.SequenceEqual ( newNegatedSet.Ranges )
                     || negatedSet.UnicodeCategoryFlagSet != newNegatedSet.UnicodeCategoryFlagSet )
                {
                    return newNegatedSet;
                }

                return negatedSet;
            }

            /// <inheritdoc />
            protected override GrammarNode<Char>? VisitStringTerminal ( StringTerminal stringTerminal, Unit argument )
            {
                if ( stringTerminal.String.Length == 1 )
                    return new CharacterTerminal ( stringTerminal.String[0] );
                else
                    return stringTerminal;
            }
        }

        private static readonly Optimizer optimizer = new Optimizer ( );

        /// <summary>
        /// Optimizes the provided tree.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static GrammarNode<Char>? Optimize ( this GrammarNode<Char> node ) =>
            optimizer.Visit ( node, default );
    }
}