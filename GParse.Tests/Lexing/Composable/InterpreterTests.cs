using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;
using GParse.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GParse.Lexing.Composable.NodeFactory;

namespace GParse.Tests.Lexing.Composable
{
    [TestClass]
    public class InterpreterTests
    {
        // [x] CharacterTerminal
        [TestMethod]
        public void Interpreter_Handles_CharacterTerminals ( )
        {
            AssertInterpretations (
                node: Terminal ( 'a' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Terminal Character
                Interpretation ( "a", true, 1 ),
                // [x] Non-terminal Character
                Interpretation ( " ", false, 0 ),
                // [x] Two Terminal Characters
                Interpretation ( "aa", true, 1 ),
                // [x] Non-terminal Character Terminal Character
                Interpretation ( " a", false, 0 ) );
        }

        // [x] NegatedCharacterTerminal
        [TestMethod]
        public void Interpreter_Handles_NegatedCharacterTerminals ( )
        {
            AssertInterpretations (
                node: !Terminal ( 'a' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Non-terminal Character
                Interpretation ( "b", true, 1 ),
                // [x] Terminal Character
                Interpretation ( "a", false, 0 ),
                // [x] Two Non-terminal Characters
                Interpretation ( "bb", true, 1 ),
                // [x] Terminal Character Non-terminal Character
                Interpretation ( "ab", false, 0 ) );
        }

        // [x] CharacterRange
        [TestMethod]
        public void Interpreter_Handles_CharacterRanges ( )
        {
            AssertInterpretations (
                node: Range ( '0', '9' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Range
                Interpretation ( "8", true, 1 ),
                // [x] Character Outside Range
                Interpretation ( "a", false, 1 ),
                // [x] Two Characters Inside Range
                Interpretation ( "88", true, 1 ),
                // [x] Character Outside Range Character Inside Range
                Interpretation ( "a8", false, 0 ) );
        }

        // [x] NegatedCharacterRange
        [TestMethod]
        public void Interpreter_Handles_NegatedCharacterRanges ( )
        {
            AssertInterpretations (
                node: !Range ( '0', '9' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Range
                Interpretation ( "a", true, 1 ),
                // [x] Character Inside Range
                Interpretation ( "5", false, 0 ),
                // [x] Two Characters Outside Range
                Interpretation ( "aa", true, 1 ),
                // [x] Character Inside Range Character Outside Range
                Interpretation ( "5a", false, 0 ) );
        }

        // [x] UnicodeCategoryTerminal
        [TestMethod]
        public void Interpreter_Handles_UnicodeCategoryTerminals ( )
        {
            AssertInterpretations (
                node: Terminal ( UnicodeCategory.LowercaseLetter ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Category
                Interpretation ( "a", true, 1 ),
                // [x] Character Outside Category
                Interpretation ( "A", false, 0 ),
                // [x] Two Characters Inside Category
                Interpretation ( "aa", true, 1 ),
                // [x] Character Outside Category Character Inside Category
                Interpretation ( "Aa", false, 0 ) );
        }

        // [x] NegatedUnicodeCategoryTerminal
        [TestMethod]
        public void Interpreter_Handles_NegatedUnicodeCategories ( )
        {
            AssertInterpretations (
                node: !Terminal ( UnicodeCategory.LowercaseLetter ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Category
                Interpretation ( "A", true, 1 ),
                // [x] Character Inside Category
                Interpretation ( "a", false, 0 ),
                // [x] Two Characters Outside Category
                Interpretation ( "AA", true, 1 ),
                // [x] Character Inside Category Character Outside Category
                Interpretation ( "aA", false, 0 ) );
        }

        // [x] StringTerminal
        [TestMethod]
        public void Interpreter_Handles_StringTerminals ( )
        {
            AssertInterpretations (
                node: Terminal ( "hello" ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Partial String
                Interpretation ( "hel", false, 0 ),
                // [x] Full String
                Interpretation ( "hello", true, 5 ),
                // [x] 2 Full Strings
                Interpretation ( "hellohello", true, 5 ),
                // [x] Different String
                Interpretation ( "there", false, 0 ),
                Interpretation ( "thello", false, 0 ),
                // [x] Different String Full String
                Interpretation ( "therehello", false, 0 ) );
        }

        // [x] Any
        [TestMethod]
        public void Interpreter_Handles_Any ( )
        {
            AssertInterpretations (
                node: Any ( ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Single Character 
                Interpretation ( "a", true, 1 ),
                // [x] Two Characters
                Interpretation ( "aa", true, 1 ) );
        }

        // [x] Set
        [TestMethod]
        public void Interpreter_Handles_Sets ( )
        {
            // [x] Characters
            AssertInterpretations (
                node: Set ( 'a', 'c' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Set
                Interpretation ( "a", true, 1 ),
                Interpretation ( "c", true, 1 ),
                // [x] Character Outside Set
                Interpretation ( "b", false, 0 ),
                // [x] Two Characters Inside Set
                Interpretation ( "aa", true, 1 ),
                Interpretation ( "cc", true, 1 ),
                // [x] Character Outside Set Character Inside Set
                Interpretation ( "ba", false, 0 ),
                Interpretation ( "bc", false, 0 ) );

            var rangeA = new Range<Char> ( 'a', 'z' );
            var rangeB = new Range<Char> ( '0', '9' );
            // [x] Ranges
            AssertInterpretations (
                node: Set ( rangeA, rangeB ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Ranges
                InterpretationRange ( 'a', 'z', true, 1 ),
                InterpretationRange ( '0', '9', true, 1 ),
                // [x] Character Outside Ranges
                Interpretation ( "/", false, 0 ),
                // [x] Two Characters Inside Ranges
                InterpretationRange ( 'a', 'z', c => $"{c}{c}", true, 1 ),
                InterpretationRange ( '0', '9', c => $"{c}{c}", true, 1 ),
                // [x] Character Outside Ranges Character Inside Ranges
                InterpretationRange ( 'a', 'z', c => $"/{c}", false, 0 ),
                InterpretationRange ( '0', '9', c => $"/{c}", false, 0 ) );

            // [x] Unicode Categories
            AssertInterpretations (
                node: Set ( UnicodeCategory.DecimalDigitNumber, UnicodeCategory.LowercaseLetter ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Categories
                InterpretationRange ( '0', '9', true, 1 ),
                InterpretationRange ( 'a', 'z', true, 1 ),
                // [x] Character Outside Categories
                Interpretation ( "A", false, 0 ),
                // [x] Two Characters Inside Categories
                InterpretationRange ( '0', '9', c => $"{c}{c}", true, 1 ),
                InterpretationRange ( 'a', 'z', c => $"{c}{c}", true, 1 ),
                // [x] Character Outside Categories Character Inside Categories
                InterpretationRange ( '0', '9', c => $"/{c}", false, 0 ),
                InterpretationRange ( 'a', 'z', c => $"/{c}", false, 0 ) );

            // [x] Nodes
            AssertInterpretations (
                node: Set ( ( !Range ( '0', '9' ) )! ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Match
                InterpretationRange ( 'a', 'z', true, 1 ),
                // [x] No Match
                InterpretationRange ( '0', '9', false, 0 ) );

            // [x] Character And Range And Unicode Category And Node
            AssertInterpretations (
                node: Set ( '/', rangeA, UnicodeCategory.UppercaseLetter, ( !Set ( CharacterClasses.NotDigit ) )! ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Inside Set
                Interpretation ( "/", true, 1 ),
                // [x] Character Inside Range
                InterpretationRange ( 'a', 'z', true, 1 ),
                // [x] Character Inside Unicode Category
                Interpretation ( "A", true, 1 ),
                // [x] Character Matched By Node
                Interpretation ( "9", true, 1 ),
                // [x] Character Outside Set, Ranges, Unicode Categories and Nodes
                Interpretation ( " ", false, 0 ) );
        }

        // [x] NegatedSet
        [TestMethod]
        public void Interpreter_Handles_NegatedSets ( )
        {
            // [x] Characters
            AssertInterpretations (
                node: !Set ( 'a', 'c' ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Set
                Interpretation ( "b", true, 1 ),
                Interpretation ( "d", true, 1 ),
                // [x] Character Inside Set
                Interpretation ( "a", false, 0 ),
                Interpretation ( "c", false, 0 ),
                // [x] Two Characters Outside Set
                Interpretation ( "bb", true, 1 ),
                Interpretation ( "dd", true, 1 ),
                // [x] Character Inside Set Character Outside Set
                Interpretation ( "ab", false, 0 ),
                Interpretation ( "cd", false, 0 ) );

            // [x] Ranges
            AssertInterpretations (
                node: !Set ( Range ( '0', '9' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Ranges
                Interpretation ( "a", true, 1 ),
                // [x] Character Inside Ranges
                Interpretation ( "5", false, 0 ),
                // [x] Two Characters Outside Ranges
                Interpretation ( "aa", true, 1 ),
                // [x] Character Inside Ranges Character Outside Ranges
                Interpretation ( "5a", false, 0 ) );

            // [x] Unicode Categories
            AssertInterpretations (
                node: !Set ( UnicodeCategory.UppercaseLetter ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Categories
                Interpretation ( "a", true, 1 ),
                // [x] Character Inside Categories
                Interpretation ( "A", false, 0 ),
                // [x] Two Characters Outside Categories
                Interpretation ( "aa", true, 1 ),
                // [x] Character Inside Categories Character Outside Categories
                Interpretation ( "Aa", false, 0 ) );

            // [x] Nodes
            AssertInterpretations (
                node: !Set ( CharacterClasses.NotDigit ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] No Match
                Interpretation ( "a", false, 0 ),
                // [x] Match
                Interpretation ( "2", true, 1 ) );

            // [x] Character And Range And Unicode Category And Node
            AssertInterpretations (
                node: !Set ( 'a', Range ( 'a', 'z' ), UnicodeCategory.LowercaseLetter, ( !Set ( CharacterClasses.Digit ) )! ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Character Outside Set
                // [x] Character Outside Range
                // [x] Character Outside Unicode Category
                // [x] Character No Matched By Node
                Interpretation ( "9", true, 1 ),
                // [x] Character Inside Set, Ranges, Unicode Categories and Nodes
                Interpretation ( "a", false, 0 ) );
        }

        // [ ] OptimizedSet
        [TestMethod]
        public void Interpreter_Handles_OptimizedSets ( )
        {
            // [x] Character Bit Vector
            AssertInterpretations (
                node: optimize ( Set ( 'a', 'c', 'e', 'g', 'i' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                Interpretation ( "a", true, 1 ),
                Interpretation ( "c", true, 1 ),
                Interpretation ( "e", true, 1 ),
                Interpretation ( "g", true, 1 ),
                Interpretation ( "i", true, 1 ),
                Interpretation ( "b", false, 0 ) );

            // [x] Character HashSet
            AssertInterpretations (
                node: optimize ( Set ( 'a', '¹' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                Interpretation ( "a", true, 1 ),
                Interpretation ( "¹", true, 1 ),
                Interpretation ( "b", false, 0 ) );

            // [x] Flattened Ranges
            AssertInterpretations (
                node: optimize ( Set ( Range ( 'a', 'z' ), Range ( '0', '9' ), Range ( 'A', 'Z' ) ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( 'a', 'z', true, 1 ),
                InterpretationRange ( '0', '9', true, 1 ),
                InterpretationRange ( 'A', 'Z', true, 1 ),
                Interpretation ( "$", false, 0 ) );

            // [x] Unicode Category FlagSet
            AssertInterpretations (
                node: optimize ( Set ( UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( 'A', 'Z', true, 1 ),
                InterpretationRange ( 'a', 'z', true, 1 ),
                Interpretation ( "1", false, 0 ) );

            // [x] Nodes
            AssertInterpretations (
                node: optimize ( Set ( !Range ( '0', '9' ), !Range ( 'a', 'z' ) ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( '0', '9', true, 1 ),
                InterpretationRange ( 'a', 'z', true, 1 ),
                Interpretation ( "$", true, 1 ) );

            static OptimizedSet optimize ( Set set ) => ( OptimizedSet ) set.Optimize ( )!;
        }

        [TestMethod]
        public void Interpreter_Handles_OptimizedNegatedSets ( )
        {
            // [x] Character Bit Vector
            AssertInterpretations (
                node: optimize ( !Set ( 'a', 'c', 'e', 'g', 'i' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                Interpretation ( "a", false, 0 ),
                Interpretation ( "c", false, 0 ),
                Interpretation ( "e", false, 0 ),
                Interpretation ( "g", false, 0 ),
                Interpretation ( "i", false, 0 ),
                Interpretation ( "b", true, 1 ) );

            // [x] Character HashSet
            AssertInterpretations (
                node: optimize ( !Set ( 'a', '¹' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                Interpretation ( "a", false, 0 ),
                Interpretation ( "¹", false, 0 ),
                Interpretation ( "b", true, 1 ) );

            // [x] Flattened Ranges
            AssertInterpretations (
                node: optimize ( !Set ( Range ( 'a', 'z' ), Range ( '0', '9' ), Range ( 'A', 'Z' ) ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( 'a', 'z', false, 0 ),
                InterpretationRange ( '0', '9', false, 0 ),
                InterpretationRange ( 'A', 'Z', false, 0 ),
                Interpretation ( "$", true, 1 ) );

            // [x] Unicode Category FlagSet
            AssertInterpretations (
                node: optimize ( !Set ( UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( 'A', 'Z', false, 0 ),
                InterpretationRange ( 'a', 'z', false, 0 ),
                Interpretation ( "1", true, 1 ) );

            // [x] Nodes
            AssertInterpretations (
                node: optimize ( !Set ( !Range ( '0', '9' ), !Range ( 'a', 'z' ) ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                InterpretationRange ( '0', '9', false, 0 ),
                InterpretationRange ( 'a', 'z', false, 0 ),
                Interpretation ( "$", false, 0 ) );

            static OptimizedNegatedSet optimize ( NegatedSet set ) => ( OptimizedNegatedSet ) set.Optimize ( )!;
        }

        // [x] NamedCapture
        [TestMethod]
        public void Interpreter_Handles_NamedCaptures ( )
        {
            AssertInterpretations (
                node: Capture ( "name", Terminal ( 'a' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Match
                Interpretation (
                    "a",
                    true,
                    1,
                    CaptureValue ( "name", 0, 1 ) ),
                // [x] No Match
                Interpretation (
                    "A",
                    false,
                    0 ) );
        }

        // [x] NamedBackreference
        [TestMethod]
        public void Interpreter_Handles_NamedBackreferences ( )
        {
            AssertInterpretations (
                node: Capture ( "name", Terminal ( 'a' ) ) & Backreference ( "name" ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: false,
                // [x] Capture Match and Backreference Match
                Interpretation (
                    "aa",
                    true,
                    2,
                    CaptureValue ( "name", 0, 1 ) ),
                // [x] Capture Match and No Backreference Match
                Interpretation (
                    "aA",
                    false,
                    0 ),
                // [x] No Capture Match and No Backreference Match
                Interpretation (
                    "AA",
                    false,
                    0 ),
                // [x] Fails With Null Captures
                Interpretation (
                    "aa",
                    false,
                    0,
                    null ) );
        }

        // [x] NumberedCapture
        [TestMethod]
        public void Interpreter_Handles_NumberedCaptures ( )
        {
            AssertInterpretations (
                node: Capture ( 1, Terminal ( 'a' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Match
                Interpretation (
                    "a",
                    true,
                    1,
                    CaptureValue ( 1, 0, 1 ) ),
                // [x] No Match
                Interpretation (
                    "A",
                    false,
                    0 ) );
        }

        // [x] NumberedBackreference
        [TestMethod]
        public void Interpreter_Handles_NumberedBackreferences ( )
        {
            AssertInterpretations (
                node: Capture ( 1, Terminal ( 'a' ) ) & Backreference ( 1 ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: false,
                // [x] Capture Match and Backreference Match
                Interpretation (
                    "aa",
                    true,
                    2,
                    CaptureValue ( 1, 0, 1 ) ),
                // [x] Capture Match and No Backreference Match
                Interpretation (
                    "aA",
                    false,
                    0 ),
                // [x] No Capture Match and No Backreference Match
                Interpretation (
                    "AA",
                    false,
                    0 ),
                // [x] Fails With Null Captures
                Interpretation (
                    "aa",
                    false,
                    0,
                    null ) );
        }

        // [x] Lookahead
        [TestMethod]
        public void Interpreter_Handles_Lookaheads ( )
        {
            AssertInterpretations (
                node: PositiveLookahead ( Terminal ( 'a' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Success
                Interpretation ( "a", true, 0 ),
                // [x] Fail
                Interpretation ( "A", false, 0 ) );

            AssertInterpretations (
                node: PositiveLookahead ( Capture ( 1, Terminal ( 'a' ) ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Success With Capture
                Interpretation (
                    "a",
                    true,
                    0,
                    CaptureValue ( 1, 0, 1 ) ),
                // [x] Fail With Capture
                Interpretation (
                    "A",
                    false,
                    0 ) );
        }

        // [x] NegativeLookahead
        [TestMethod]
        public void Interpreter_Handles_NegativeLookaheads ( )
        {
            AssertInterpretations (
                node: NegativeLookahead ( Terminal ( 'a' ) ),
                shouldNotMatchEmpty: false,
                shouldWorkWithoutCaptures: true,
                // [x] Success
                Interpretation (
                    "A",
                    true,
                    0 ),
                // [x] Fail
                Interpretation (
                    "a",
                    false,
                    0 ) );

            // There's no point in adding a capture to a negative
            // lookahead but we gotta make sure that captures aren't
            // stored.
            AssertInterpretations (
                node: NegativeLookahead ( Capture ( 1, Terminal ( 'a' ) ) ),
                shouldNotMatchEmpty: false,
                shouldWorkWithoutCaptures: true,
                // [x] Success With Capture
                Interpretation (
                    "A",
                    true,
                    0 ),
                // [x] Fail With Capture
                Interpretation (
                    "a",
                    false,
                    0 ) );
        }

        // [x] Sequence
        [TestMethod]
        public void Interpreter_Handles_Sequences ( )
        {
            AssertInterpretations (
                node: Sequence ( Capture ( 1, Any ( ) ), Terminal ( 'a' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Partial Match
                // [x] Partial Match With Capture
                Interpretation ( "ab", false, 0 ),
                // [x] Full Match
                // [x] Full Match With Capture
                Interpretation ( "aa", true, 2, CaptureValue ( 1, 0, 1 ) ) );
        }

        // [x] Alternation
        [TestMethod]
        public void Interpreter_Handles_Alternations ( )
        {
            AssertInterpretations (
                node: ( Capture ( 1, Terminal ( 'a' ) ) & Terminal ( 'a' ) )
                      | ( Capture ( 2, Terminal ( 'b' ) ) & Terminal ( 'b' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] First Matched
                Interpretation (
                    "aa",
                    true,
                    2,
                    CaptureValue ( 1, 0, 1 ) ),
                // [x] Second Matched
                Interpretation (
                    "bb",
                    true,
                    2,
                    CaptureValue ( 2, 0, 1 ) ) );
            AssertInterpretations (
                node: ( Capture ( 1, Terminal ( 'b' ) ) & Terminal ( 'a' ) )
                      | ( Capture ( 2, Terminal ( 'b' ) ) & Terminal ( 'b' ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Second Matched but First had Captures
                Interpretation (
                    "bb",
                    true,
                    2,
                    CaptureValue ( 2, 0, 1 ) )
                );
        }

        // [x] Repetition
        [TestMethod]
        public void Interpreter_Handles_Repetitions ( )
        {
            // [x] Lazy Throws Exception
            Assert.ThrowsException<NotSupportedException> ( ( ) =>
                AssertInterpretation (
                    Repetition ( Terminal ( 'a' ), 1, 5 ).Lazily ( ),
                    "",
                    false,
                    0 ) );

            // [x] Without Minimum Without Maximum
            AssertInterpretations (
                node: Repetition ( Terminal ( 'a' ), 0, null ),
                shouldNotMatchEmpty: false,
                shouldWorkWithoutCaptures: true,
                // [x] 0 Matches
                Interpretation ( "", true, 0 ),
                // [x] 1 Match
                Interpretation ( "a", true, 1 ),
                // [x] 2 Matches
                Interpretation ( "aa", true, 2 ) );

            // [x] Minimum 0 With Maximum
            AssertInterpretations (
                node: Repetition ( Terminal ( 'a' ), 0, 3 ),
                shouldNotMatchEmpty: false,
                shouldWorkWithoutCaptures: true,
                // [x] Below Maximum
                Interpretation ( "aaa", true, 3 ),
                // [x] Above Maximum
                Interpretation ( "aaaaa", true, 3 ) );

            // [x] Minimum > 0 Without Maximum
            AssertInterpretations (
                node: Repetition ( Capture ( 1, Terminal ( 'a' ) ), 2, null ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                // [x] Below Minimum With Capture
                Interpretation ( "a", false, 0 ),
                // [x] Above Minimum With Capture
                Interpretation ( "aaaa", true, 4, CaptureValue ( 1, 3, 1 ) ) );

            // [x] Minimum > 0 With Maximum
            AssertInterpretations (
                node: Repetition ( Capture ( 1, Any ( ).Optional ( ) ), 2, 5 ),
                shouldWorkWithoutCaptures: true,
                // [x] Empty Input
                Interpretation ( "", true, 0, CaptureValue ( 1, 0, 0 ) ),
                // [x] Below Minimum With Capture With Empty Match For The Remaining Matches
                Interpretation ( "a", true, 1, CaptureValue ( 1, 1, 0 ) ),
                // [x] Above Minimum With Capture With Empty Match For The Remaining Matches
                Interpretation ( "aa", true, 2, CaptureValue ( 1, 2, 0 ) ),
                // [x] Above Maximum
                Interpretation ( "aaaaaa", true, 5, CaptureValue ( 1, 4, 1 ) ) );
        }

        [TestMethod]
        public void Interpeter_Handles_CompleteExpressions ( )
        {
            AssertInterpretations (
                node: Terminal ( "--" ) & Capture ( 1, Repetition ( CharacterClasses.Dot, 0, null ) ),
                shouldNotMatchEmpty: true,
                shouldWorkWithoutCaptures: true,
                Interpretation ( "", false, 0 ),
                Interpretation ( "--\na", true, 2, CaptureValue ( 1, 2, 0 ) ) );
        }

        #region Helper stuff

        private static InterpretationParams Interpretation ( String input, Boolean isMatch, Int32 length, params KeyValuePair<String, Capture>[]? captures ) =>
            new ( input, isMatch, length, captures );

        private static KeyValuePair<String, Capture> CaptureValue ( String name, Int32 start, Int32 length ) =>
            new ( name, new ( start, length ) );

        private static KeyValuePair<String, Capture> CaptureValue ( Int32 position, Int32 start, Int32 length ) =>
            new ( GrammarTreeInterpreter.GetCaptureKey ( position ), new ( start, length ) );

        private static InterpretationsList InterpretationRange ( Char start, Char end, Boolean isMatch, Int32 length ) =>
            InterpretationRange ( start, end, ch => ch.ToString ( ), isMatch, length );

        private static InterpretationsList InterpretationRange ( Char start, Char end, Func<Char, String> inputGenerator, Boolean isMatch, Int32 length ) =>
            new ( Enumerable.Range ( start, end - start + 1 )
                                             .Select ( c => ( Char ) c )
                                             .Select ( inputGenerator )
                                             .Select ( input => Interpretation ( input, isMatch, length ) ) );

        private readonly struct InterpretationsList
        {
            public InterpretationsList ( InterpretationParams interpretationParams )
                : this ( Enumerable.Repeat ( interpretationParams, 1 ) )
            {
            }

            public InterpretationsList ( IEnumerable<InterpretationParams> interpretationParams )
            {
                this.InterpretationParams = interpretationParams;
            }

            public IEnumerable<InterpretationParams> InterpretationParams { get; }

            public static implicit operator InterpretationsList ( InterpretationParams interpretationParams ) =>
                new ( interpretationParams );
        }

        private class InterpretationParams
        {
            public InterpretationParams ( String input, Boolean isMatch, Int32 length, KeyValuePair<String, Capture>[]? captures )
            {
                this.Input = input ?? throw new ArgumentNullException ( nameof ( input ) );
                this.IsMatch = isMatch;
                this.Length = length;
                this.Captures = captures;
            }

            public String Input { get; }
            public Boolean IsMatch { get; }
            public Int32 Length { get; }
            public KeyValuePair<String, Capture>[]? Captures { get; }

            public void Deconstruct ( out String input, out Boolean isMatch, out Int32 length, out KeyValuePair<String, Capture>[]? captures )
            {
                input = this.Input;
                isMatch = this.IsMatch;
                length = this.Length;
                captures = this.Captures;
            }
        }

        private static void AssertInterpretations (
            GrammarNode<Char> node,
            Boolean shouldNotMatchEmpty,
            Boolean shouldWorkWithoutCaptures,
            params InterpretationsList[] interpretationsLists )
        {
            var interpretationsListsList = new List<InterpretationsList>
            {
                Interpretation ( "", !shouldNotMatchEmpty, 0 )
            };
            interpretationsListsList.AddRange ( interpretationsLists );
            AssertInterpretations ( node, shouldWorkWithoutCaptures, interpretationsListsList.ToArray ( ) );
        }

        private static void AssertInterpretations (
            GrammarNode<Char> node,
            Boolean shouldWorkWithoutCaptures,
            params InterpretationsList[] interpretationsLists )
        {
            foreach ( InterpretationsList interpretations in interpretationsLists )
            {
                foreach ( (var input, var isMatch, var length, KeyValuePair<String, Capture>[]? expectedCaptures) in interpretations.InterpretationParams )
                {
                    AssertInterpretation ( node, input, isMatch, length, expectedCaptures );
                    if ( shouldWorkWithoutCaptures )
                        AssertInterpretation ( node, input, isMatch, length, null );
                }
            }
        }

        private static void AssertInterpretation (
            GrammarNode<Char> node,
            String input,
            Boolean isMatch,
            Int32 length,
            params KeyValuePair<String, Capture>[]? expectedCaptures )
        {
            // Setup
            var reader = new StringCodeReader ( input );
            Dictionary<String, Capture>? actualCaptures = expectedCaptures is null ? null : new Dictionary<String, Capture> ( );

            // Act
            SimpleMatch result = GrammarTreeInterpreter.MatchSimple ( reader, node, actualCaptures );

            // Check
            Assert.AreEqual ( isMatch, result.IsMatch, $"Expected interpreter to {( isMatch ? "match" : "not match" )} input '{input}' but it {( result.IsMatch ? "has" : "hasn't" )}." );
            if ( isMatch )
            {
                Assert.AreEqual ( length, result.Length, $"Expected interpreter to consume {length} characters for input '{input}' but it has consumed {result.Length}." );
            }

            if ( actualCaptures is not null )
            {
                KeyValuePair<String, Capture>[] actualKeyValuePairs = actualCaptures.ToArray ( );
                CollectionAssert.AreEquivalent ( expectedCaptures, actualKeyValuePairs, $"Captures did not match for input '{input}'." );
            }
        }

        #endregion Helper stuff
    }
}
