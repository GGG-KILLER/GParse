using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
using GParse.IO;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The class that contains the logic for matching grammars nodes on a <see
    /// cref="IReadOnlyCodeReader" />
    /// </summary>
    public static class GrammarTreeInterpreter
    {
        private readonly struct InterpreterState
        {
            public IReadOnlyCodeReader Reader { get; }
            public Int32 Offset { get; }
            public IDictionary<String, Capture>? Captures { get; }
            public GrammarNode<Char>? Next { get; }

            public InterpreterState ( IReadOnlyCodeReader reader, Int32 offset, IDictionary<String, Capture>? captures = null, GrammarNode<Char>? next = null )
            {
                this.Reader = reader;
                this.Offset = offset;
                this.Captures = captures;
                this.Next = next;
            }
        }

        private class Interpreter : GrammarTreeVisitor<SimpleMatch, InterpreterState>
        {
            public static readonly Interpreter Instance = new Interpreter ( );

            private static SimpleMatch MatchWithTempCaptures ( InterpreterState state, Func<InterpreterState, SimpleMatch> func )
            {
                IDictionary<String, Capture>? captures = state.Captures;
                if ( captures is null )
                {
                    return func ( state );
                }
                else
                {
                    var tempCaptures = new Dictionary<String, Capture> ( captures );
                    SimpleMatch match = func ( new InterpreterState ( state.Reader, state.Offset, tempCaptures, state.Next ) );
                    if ( match.IsMatch )
                    {
                        foreach ( KeyValuePair<String, Capture> kv in tempCaptures )
                            captures[kv.Key] = kv.Value;
                    }
                    return match;
                }
            }

            protected override SimpleMatch VisitAlternation ( Alternation<Char> alternation, InterpreterState argument ) =>
                alternation.GrammarNodes.Select ( node => MatchWithTempCaptures ( argument, argument => this.Visit ( node, argument ) ) ).FirstOrDefault ( match => match.IsMatch );

            protected override SimpleMatch VisitCharacterRange ( CharacterRange characterRange, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is Char ch && CharUtils.IsInRange ( characterRange.Range.Start, ch, characterRange.Range.End )
                ? SimpleMatch.SingleChar
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitCharacterTerminal ( CharacterTerminal characterTerminal, InterpreterState argument ) =>
                argument.Reader.IsAt ( characterTerminal.Value, argument.Offset )
                ? SimpleMatch.SingleChar
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitRepetition ( Repetition<Char> repetition, InterpreterState argument )
            {
                if ( repetition.IsLazy )
                    throw new NotSupportedException ( "Lazy repetitions aren't supported yet." );

                return MatchWithTempCaptures ( argument, argument =>
                {
                    Int32 matches = 0, totalLength = 0;
                    SimpleMatch test;

                    IReadOnlyCodeReader reader = argument.Reader;
                    var offset = argument.Offset;
                    IDictionary<String, Capture>? captures = argument.Captures;
                    GrammarNode<Char>? next = argument.Next;

                    GrammarNode<Char> innerNode = repetition.InnerNode;
                    var minimum = repetition.Range.Minimum;
                    var maximum = repetition.Range.Maximum;

                    do
                    {
                        test = this.Visit ( innerNode, new InterpreterState ( reader, offset + totalLength, captures, next ) );

                        if ( test.IsMatch )
                            matches++;
                        totalLength += test.Length;
                    }
                    while ( test.IsMatch
                            // Repetitions don't match empty matches more than once unless if necessary.
                            && ( test.Length > 0 || ( minimum is not null && matches < minimum ) )
                            && ( maximum is null || matches < maximum ) );

                    if ( minimum is null || minimum <= matches )
                        return new SimpleMatch ( true, totalLength );
                    else
                        return SimpleMatch.Fail;
                } );
            }

            protected override SimpleMatch VisitLookahead ( Lookahead lookahead, InterpreterState argument ) =>
                MatchWithTempCaptures ( argument, argument => new SimpleMatch ( this.Visit ( lookahead.InnerNode, argument ).IsMatch, 0 ) );

            private static SimpleMatch VisitBackreference ( String name, InterpreterState argument )
            {
                if ( argument.Captures is not null && argument.Captures.TryGetValue ( name, out Capture capture ) )
                {
                    ReadOnlySpan<Char> value = argument.Reader.PeekSpan ( capture.Length, capture.Start );
                    if (
                        !value.IsEmpty
                        && argument.Reader.IsAt ( value, argument.Offset ) )
                    {
                        return new SimpleMatch ( true, value.Length );
                    }
                }
                return SimpleMatch.Fail;
            }

            protected override SimpleMatch VisitNamedBackreference ( NamedBackreference namedBackreference, InterpreterState argument ) =>
                VisitBackreference ( namedBackreference.Name, argument );

            protected override SimpleMatch VisitNamedCapture ( NamedCapture namedCapture, InterpreterState argument )
            {
                SimpleMatch res = this.Visit ( namedCapture.InnerNode, argument );
                if ( res.IsMatch && argument.Captures is not null )
                    argument.Captures[namedCapture.Name] = new Capture ( argument.Offset, res.Length );
                return res;
            }

            protected override SimpleMatch VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is Char ch && !CharUtils.IsInRange ( negatedCharacterRange.Range.Start, ch, negatedCharacterRange.Range.End )
                ? SimpleMatch.SingleChar
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is Char ch && ch != negatedCharacterTerminal.Value
                ? new SimpleMatch ( true, 0 )
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is Char ch && Char.GetUnicodeCategory ( ch ) != negatedUnicodeCategoryTerminal.Category
                ? new SimpleMatch ( true, 0 )
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitNegativeLookahead ( NegativeLookahead negativeLookahead, InterpreterState argument ) =>
                MatchWithTempCaptures ( argument, argument => new SimpleMatch ( !this.Visit ( negativeLookahead.InnerNode, argument ).IsMatch, 0 ) );

            protected override SimpleMatch VisitNumberedBackreference ( NumberedBackreference numberedBackreference, InterpreterState argument ) =>
                VisitBackreference ( $"<{numberedBackreference.Position}>", argument );

            protected override SimpleMatch VisitNumberedCapture ( NumberedCapture numberedCapture, InterpreterState argument ) =>
                MatchWithTempCaptures ( argument, argument =>
                {
                    SimpleMatch res = this.Visit ( numberedCapture.InnerNode, argument );
                    if ( res.IsMatch && argument.Captures is not null )
                        argument.Captures[$"<{numberedCapture.Position}>"] = new Capture ( argument.Offset, res.Length );
                    return res;
                } );

            protected override SimpleMatch VisitSequence ( Sequence<Char> sequence, InterpreterState argument )
            {
                return MatchWithTempCaptures ( argument, argument =>
                {
                    IReadOnlyCodeReader reader = argument.Reader;
                    var offset = argument.Offset;
                    IDictionary<String, Capture>? captures = argument.Captures;
                    var totalLength = 0;

                    ImmutableArray<GrammarNode<Char>> nodes = sequence.GrammarNodes;
                    for ( var idx = 0; idx < nodes.Length; idx++ )
                    {
                        GrammarNode<Char>? node = nodes[idx];
                        SimpleMatch res = this.Visit ( node, new InterpreterState ( reader, offset + totalLength, captures ) );
                        if ( !res.IsMatch )
                            return default;
                        totalLength += res.Length;
                    }
                    return new SimpleMatch ( true, totalLength );
                } );
            }

            protected override SimpleMatch VisitStringTerminal ( StringTerminal characterTerminalString, InterpreterState argument ) =>
                argument.Reader.IsAt ( characterTerminalString.String, argument.Offset )
                ? new SimpleMatch ( true, characterTerminalString.String.Length )
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is Char ch && Char.GetUnicodeCategory ( ch ) == unicodeCategoryTerminal.Category
                ? SimpleMatch.SingleChar
                : SimpleMatch.Fail;

            public SimpleMatch Visit ( GrammarNode<Char> node, IReadOnlyCodeReader reader, IDictionary<String, Capture>? captures = null ) =>
                this.Visit ( node, new InterpreterState ( reader, 0, captures, null ) );

            protected override SimpleMatch VisitAny ( Any any, InterpreterState argument ) =>
                argument.Reader.Peek ( argument.Offset ) is not null
                ? SimpleMatch.SingleChar
                : SimpleMatch.Fail;

            protected override SimpleMatch VisitSet ( Set set, InterpreterState argument )
            {
                if ( argument.Reader.Peek ( argument.Offset ) is not Char peek )
                    return SimpleMatch.Fail;

                if ( set.Characters.Contains ( peek ) )
                    return SimpleMatch.SingleChar;

                ImmutableArray<Char> ranges = set.FlattenedRanges;
                if ( ranges.Length > 0
                     && ( ranges.Length == 2
                        ? CharUtils.IsInRange ( ranges[0], peek, ranges[1] )
                        : CharUtils.IsInRanges ( ranges, peek ) ) )
                {
                    return SimpleMatch.SingleChar;
                }

                var flagSet = set.UnicodeCategoryFlagSet;
                if ( flagSet != 0 && CharUtils.IsCategoryInSet ( flagSet, Char.GetUnicodeCategory ( peek ) ) )
                    return SimpleMatch.SingleChar;

                // The set nodes shouldn't have any backreferences nor captures so we don't pass the captures to it.
                var nodesArgument = new InterpreterState ( argument.Reader, argument.Offset );
                if ( set.Nodes.Any ( node => this.Visit ( node, nodesArgument ).IsMatch ) )
                    return SimpleMatch.SingleChar;

                return SimpleMatch.Fail;
            }

            protected override SimpleMatch VisitNegatedSet ( NegatedSet negatedSet, InterpreterState argument )
            {
                if ( argument.Reader.Peek ( argument.Offset ) is not Char peek )
                    return SimpleMatch.Fail;

                if ( negatedSet.Characters.Contains ( peek ) )
                    return SimpleMatch.Fail;

                ImmutableArray<Char> ranges = negatedSet.FlattenedRanges;
                if ( ranges.Length > 0
                     && ( ranges.Length == 2
                        ? CharUtils.IsInRange ( ranges[0], peek, ranges[1] )
                        : CharUtils.IsInRanges ( ranges, peek ) ) )
                {
                    return SimpleMatch.Fail;
                }

                var flagSet = negatedSet.UnicodeCategoryFlagSet;
                if ( flagSet != 0 && CharUtils.IsCategoryInSet ( flagSet, Char.GetUnicodeCategory ( peek ) ) )
                    return SimpleMatch.Fail;

                // The set nodes shouldn't have any backreferences nor captures so we don't pass the captures to it.
                var nodesArgument = new InterpreterState ( argument.Reader, argument.Offset );
                if ( negatedSet.Nodes.Any ( node => this.Visit ( node, nodesArgument ).IsMatch ) )
                    return SimpleMatch.Fail;

                return SimpleMatch.SingleChar;
            }
        }

        /// <summary>
        /// Executes the match process on the code reader and returns a <see cref="Composable.SimpleMatch"/>.
        /// </summary>
        /// <param name="reader">The reader to match over.</param>
        /// <param name="node">The node rule to match.</param>
        /// <param name="captures">The dictionary to store tha capture groups at.</param>
        /// <returns></returns>
        public static SimpleMatch Simple ( IReadOnlyCodeReader reader, GrammarNode<Char> node, IDictionary<String, Capture>? captures = null ) =>
            Interpreter.Instance.Visit ( node, reader, captures );

        /// <summary>
        /// Executes the match process on the code reader and returns a <see
        /// cref="Composable.SpanMatch" />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static SpanMatch Span ( ICodeReader reader, GrammarNode<Char> node )
        {
            var captures = new Dictionary<String, Capture> ( );
            (var isMatch, var length) = Interpreter.Instance.Visit ( node, reader, captures );
            return isMatch
                   ? new SpanMatch ( true, reader.ReadSpan ( length ), captures )
                   : new SpanMatch ( false, default, default );
        }

        /// <summary>
        /// Executes the match process on the code reader and returns a <see
        /// cref="Composable.StringMatch" />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static StringMatch String ( ICodeReader reader, GrammarNode<Char> node )
        {
            var captures = new Dictionary<String, Capture> ( );
            (var isMatch, var length) = Interpreter.Instance.Visit ( node, reader, captures );
            return isMatch
                   ? new StringMatch ( true, reader.ReadString ( length ), captures )
                   : new StringMatch ( false, default, default );
        }
    }
}