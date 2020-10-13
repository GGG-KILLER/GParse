using System;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Contains common character classes.
    /// </summary>
    public static partial class RegexUtils
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
        /// Matches any word characters ([A-Za-z0-9_]).
        /// </summary>
        public static readonly Set Word = new Set (
            '_',
            new Range<Char> ( 'A', 'Z' ),
            new Range<Char> ( 'a', 'z' ),
            new Range<Char> ( '0', '9' ) );

        /// <summary>
        /// Matches whitespace ([ \f\n\r\t\v]).
        /// </summary>
        public static readonly Set Whitespace = new Set ( ' ', '\f', '\n', '\r', '\t', '\v' );
    }
}
