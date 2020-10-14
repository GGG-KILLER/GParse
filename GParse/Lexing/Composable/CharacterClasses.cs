using System;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Contains common character classes.
    /// </summary>
    public static partial class CharacterClasses
    {
        /// <summary>
        /// Matches any character except newlines ([^\n]).
        /// </summary>
        public static readonly NegatedCharacterTerminal Dot = new ( '\n' );

        /// <summary>
        /// Matches any arabic numerals ([0-9]).
        /// </summary>
        public static readonly CharacterRange Digit = new ( '0', '9' );

        /// <summary>
        /// Matches any non arabic numeral characters ([^0-9]).
        /// </summary>
        public static readonly NegatedCharacterRange NotDigit = !Digit;

        /// <summary>
        /// Matches any word characters ([A-Za-z0-9_]).
        /// </summary>
        public static readonly Set Word = new Set (
            '_',
            new Range<Char> ( 'A', 'Z' ),
            new Range<Char> ( 'a', 'z' ),
            new Range<Char> ( '0', '9' ) );

        /// <summary>
        /// Matches any non word characters ([A-Za-z0-9_]).
        /// </summary>
        public static readonly NegatedSet NotWord = !Word;

        /// <summary>
        /// Matches any whitespace characters ([ \f\n\r\t\v]).
        /// </summary>
        public static readonly Set Whitespace = new Set ( ' ', '\f', '\n', '\r', '\t', '\v' );

        /// <summary>
        /// Matches any non whitespace characters ([ \f\n\r\t\v]).
        /// </summary>
        public static readonly NegatedSet NotWhitespace = !Whitespace;
    }
}
