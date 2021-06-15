using System;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
using Tsu;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The class responsible for obtaining the constant prefix for a grammar tree.
    /// </summary>
    public static class GrammarTreePrefixObtainer
    {
        private sealed class Obtainer : GrammarTreeVisitor<String?, Unit>
        {
            protected override String? VisitAlternation(Alternation<Char> alternation, Unit argument) => null;
            protected override String? VisitAny(Any any, Unit argument) => null;
            protected override String? VisitCharacterRange(CharacterRange characterRange, Unit argument) => null;
            protected override String? VisitNamedBackreference(NamedBackreference namedBackreference, Unit argument) => null;
            protected override String? VisitNegatedCharacterRange(NegatedCharacterRange negatedCharacterRange, Unit argument) => null;
            protected override String? VisitNegatedCharacterTerminal(NegatedCharacterTerminal negatedCharacterTerminal, Unit argument) => null;
            protected override String? VisitNegatedSet(NegatedSet negatedSet, Unit argument) => null;
            protected override String? VisitNegatedUnicodeCategoryTerminal(NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, Unit argument) => null;
            protected override String? VisitNegativeLookahead(NegativeLookahead negativeLookahead, Unit argument) => null;
            protected override String? VisitNumberedBackreference(NumberedBackreference numberedBackreference, Unit argument) => null;
            protected override String? VisitSet(Set set, Unit argument) => null;
            protected override String? VisitUnicodeCategoryTerminal(UnicodeCategoryTerminal unicodeCategoryTerminal, Unit argument) => null;
            protected override String? VisitOptimizedSet(OptimizedSet optimizedSet, Unit argument) => null;
            protected override String? VisitOptimizedNegatedSet(OptimizedNegatedSet optimizedNegatedSet, Unit argument) => null;

            protected override String? VisitCharacterTerminal(CharacterTerminal characterTerminal, Unit argument) =>
                Char.ToString(characterTerminal.Value);
            protected override String? VisitPositiveLookahead(PositiveLookahead positiveLookahead, Unit argument) =>
                this.Visit(positiveLookahead.InnerNode, default);
            protected override String? VisitNamedCapture(NamedCapture namedCapture, Unit argument) =>
                this.Visit(namedCapture.InnerNode, default);
            protected override String? VisitNumberedCapture(NumberedCapture numberedCapture, Unit argument) =>
                this.Visit(numberedCapture.InnerNode, default);
            protected override String? VisitStringTerminal(StringTerminal stringTerminal, Unit argument) =>
                stringTerminal.Value;

            protected override String? VisitRepetition(Repetition<Char> repetition, Unit argument)
            {
                var element = this.Visit(repetition.InnerNode, default);
                if (String.IsNullOrEmpty(element))
                    return null;
                return String.Concat(Enumerable.Repeat(element, (Int32) repetition.Range.Minimum));
            }

            protected override String? VisitSequence(Sequence<Char> sequence, Unit argument)
            {
                var prefixes = sequence.GrammarNodes.Select(node => this.Visit(node, default))
                                                    .TakeWhile(str => !String.IsNullOrEmpty(str))
                                                    .ToImmutableArray();
                if (prefixes.IsEmpty)
                    return null;
                return String.Concat(prefixes);
            }
        }

        private static readonly Obtainer _generator = new();

        /// <summary>
        /// Generates the prefix of the provided tree.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        public static String? Calculate(GrammarNode<Char> grammarNode) =>
            _generator.Visit(grammarNode, default);
    }
}