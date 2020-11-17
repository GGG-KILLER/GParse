using System;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A <see cref="GrammarNode{Char}"/> factory.
    /// </summary>
    public static class NodeFactory
    {
        /// <summary>
        /// Creates a new <see cref="Any"/>.
        /// </summary>
        /// <returns></returns>
        public static Any Any ( ) => new ( );

        /// <summary>
        /// Creates a new <see cref="CharacterTerminal"/>.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static CharacterTerminal Terminal ( Char ch ) => new ( ch );

        /// <summary>
        /// Creates a new <see cref="StringTerminal"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringTerminal Terminal ( String str ) => new ( str );

        /// <summary>
        /// Creates a new <see cref="UnicodeCategoryTerminal"/>.
        /// </summary>
        /// <param name="unicodeCategory"></param>
        /// <returns></returns>
        public static UnicodeCategoryTerminal Terminal ( UnicodeCategory unicodeCategory ) => new ( unicodeCategory );

        /// <summary>
        /// Creates a new <see cref="CharacterRange"/>.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static CharacterRange Range ( Char start, Char end ) => new ( start, end );

        /// <summary>
        /// Creates a new <see cref="Composable.Set"/>.
        /// </summary>
        /// <param name="setElements"></param>
        /// <returns></returns>
        public static Set Set ( params SetElement[] setElements ) => new ( setElements );

        /// <summary>
        /// Creates a new <see cref="NumberedBackreference"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static NumberedBackreference Backreference ( Int32 position ) => new ( position );

        /// <summary>
        /// Creates a new <see cref="NamedBackreference"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamedBackreference Backreference ( String name ) => new ( name );

        /// <summary>
        /// Creates a new <see cref="NumberedCapture"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="innerNode"></param>
        /// <returns></returns>
        public static NumberedCapture Capture ( Int32 position, GrammarNode<Char> innerNode ) => new ( position, innerNode );

        /// <summary>
        /// Creates a new <see cref="NamedCapture"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="innerNode"></param>
        /// <returns></returns>
        public static NamedCapture Capture ( String name, GrammarNode<Char> innerNode ) => new ( name, innerNode );

        /// <summary>
        /// Creates a new <see cref="Composable.Lookahead"/>.
        /// </summary>
        /// <param name="innerNode"></param>
        /// <returns></returns>
        public static Lookahead Lookahead ( GrammarNode<Char> innerNode ) => new ( innerNode );

        /// <summary>
        /// Creates a new <see cref="Sequence{T}"/>.
        /// </summary>
        /// <param name="grammarNodes"></param>
        /// <returns></returns>
        public static Sequence<Char> Sequence ( params GrammarNode<Char>[] grammarNodes ) => new ( grammarNodes );

        /// <summary>
        /// Creates a new <see cref="Alternation{T}"/>.
        /// </summary>
        /// <param name="grammarNodes"></param>
        /// <returns></returns>
        public static Alternation<Char> Alternation ( params GrammarNode<Char>[] grammarNodes ) => new ( grammarNodes );

        /// <summary>
        /// Creates a new <see cref="Repetition{T}"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="repetitionRange"></param>
        /// <returns></returns>
        public static Repetition<Char> Repetition ( GrammarNode<Char> node, RepetitionRange repetitionRange ) =>
            new ( node, repetitionRange, false );
    }
}
