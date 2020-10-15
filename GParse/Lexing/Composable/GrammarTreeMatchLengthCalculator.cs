using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Calculates the minimum and maximum match length for a grammar tree.
    /// </summary>
    public static class GrammarTreeMatchLengthCalculator

    {
        /// <summary>
        /// The options to be used by the calculator.
        /// </summary>
        public readonly struct LengthCalculatorOptions
        {
            /// <summary>
            /// Whether to use a length of 0 for lookaheads.
            /// </summary>
            public Boolean ZeroLengthLookahead { get; }

            /// <summary>
            /// The length to be reported for backreferences.
            /// </summary>
            public UInt32 BackreferenceLength { get; }

            /// <summary>
            /// Initializes a new options struct.
            /// </summary>
            /// <param name="useMatchResultLength"></param>
            /// <param name="backreferenceLength"></param>
            public LengthCalculatorOptions ( Boolean useMatchResultLength, UInt32 backreferenceLength )
            {
                this.ZeroLengthLookahead = useMatchResultLength;
                this.BackreferenceLength = backreferenceLength;
            }
        }

        private class LengthCalculator : GrammarTreeVisitor<Range<UInt32>, LengthCalculatorOptions>
        {
            protected override Range<UInt32> VisitAlternation ( Alternation<Char> alternation, LengthCalculatorOptions argument )
            {
                IEnumerable<Range<UInt32>> ranges =
                    alternation.GrammarNodes.Select ( node => this.Visit ( node, argument ) );
                return new Range<UInt32> ( ranges.Min ( r => r.Start ), ranges.Max ( r => r.End ) );
            }

            protected override Range<UInt32> VisitAny ( Any any, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitCharacterRange ( CharacterRange characterRange, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitSet ( Set set, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitCharacterTerminal ( CharacterTerminal characterTerminal, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitLookahead ( Lookahead lookahead, LengthCalculatorOptions argument )
            {
                if ( argument.ZeroLengthLookahead )
                    return new Range<UInt32> ( 0 );
                return this.Visit ( lookahead.InnerNode, argument );
            }

            protected override Range<UInt32> VisitNamedBackreference ( NamedBackreference namedBackreference, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( argument.BackreferenceLength );

            protected override Range<UInt32> VisitNamedCapture ( NamedCapture namedCapture, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( argument.BackreferenceLength );

            protected override Range<UInt32> VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitNegatedSet ( NegatedSet negatedCharacterSet, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );

            protected override Range<UInt32> VisitNegativeLookahead ( NegativeLookahead negativeLookahead, LengthCalculatorOptions argument )
            {
                if ( argument.ZeroLengthLookahead )
                    return new Range<UInt32> ( 0 );
                return this.Visit ( negativeLookahead.InnerNode, argument );
            }

            protected override Range<UInt32> VisitNumberedBackreference ( NumberedBackreference numberedBackreference, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( argument.BackreferenceLength );

            protected override Range<UInt32> VisitNumberedCapture ( NumberedCapture numberedCapture, LengthCalculatorOptions argument ) =>
                this.Visit ( numberedCapture.InnerNode, argument );

            protected override Range<UInt32> VisitRepetition ( Repetition<Char> repetition, LengthCalculatorOptions argument )
            {
                Range<UInt32> range = this.Visit ( repetition.InnerNode, argument );
                return new Range<UInt32> (
                    SaturatingMath.Multiply ( repetition.Range.Minimum ?? UInt32.MinValue, range.Start ),
                    SaturatingMath.Multiply ( repetition.Range.Maximum ?? UInt32.MaxValue, range.End ) );
            }

            protected override Range<UInt32> VisitSequence ( Sequence<Char> sequence, LengthCalculatorOptions argument )
            {
                var min = 0U;
                var max = 0U;
                foreach ( Range<UInt32> range in sequence.GrammarNodes.Select ( node => this.Visit ( node, argument ) ) )
                {
                    min = SaturatingMath.Add ( range.Start, min );
                    max = SaturatingMath.Add ( range.End, max );
                }
                return new Range<UInt32> ( min, max );
            }

            protected override Range<UInt32> VisitStringTerminal ( StringTerminal characterTerminalString, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( ( UInt32 ) characterTerminalString.String.Length );

            protected override Range<UInt32> VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, LengthCalculatorOptions argument ) =>
                new Range<UInt32> ( 1 );
        }

        private static readonly LengthCalculator calculator = new LengthCalculator ( );

        /// <summary>
        /// Calculates the length of the provided grammar node according to the provided options.
        /// </summary>
        /// <param name="node">The node length to be calculated.</param>
        /// <param name="options">The options to use when calculating the length.</param>
        /// <returns></returns>
        public static Range<UInt32> Calculate ( GrammarNode<Char> node, LengthCalculatorOptions options ) =>
            calculator.Visit ( node, options );
    }
}
