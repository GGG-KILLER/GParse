using System;
using GParse.Composable;

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
        public static readonly Alternation<Char> Word =  new CharacterTerminal ( '_' )
            | new CharacterRange ( 'A', 'Z' )
            | new CharacterRange ( 'a', 'z' )
            | new CharacterRange ( '0', '9' );

        /// <summary>
        /// Matches whitespace ([ \f\n\r\t\v]).
        /// </summary>
        public static readonly CharacterSet Whitespace = new ( " \f\n\r\t\v" );
    }
}
