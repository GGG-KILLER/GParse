using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that matches a set of characters.
    /// </summary>
    public class CharacterSet : GrammarNode<Char>
    {
        /// <summary>
        /// The set of characters this node matches.
        /// </summary>
        public IImmutableSet<Char> CharSet { get; }

        /// <summary>
        /// Initializes this character set.
        /// </summary>
        /// <param name="charSet"></param>
        public CharacterSet ( params Char[] charSet ) : this ( ( IEnumerable<Char> ) charSet )
        {
        }

        /// <summary>
        /// Initializes this character set.
        /// </summary>
        /// <param name="charSet"></param>
        public CharacterSet ( IEnumerable<Char> charSet )
        {
            if ( charSet is null )
                throw new ArgumentNullException ( nameof ( charSet ) );
            if ( !charSet.Any ( ) )
                throw new ArgumentException ( "The input charset must not be empty" );

            this.CharSet = charSet.ToImmutableHashSet ( );
        }

        /// <summary>
        /// The conversion from arrays to a char set node
        /// </summary>
        /// <param name="charSet"></param>
        public static implicit operator CharacterSet ( Char[] charSet ) =>
            new CharacterSet ( charSet );

        /// <summary>
        /// Negates this charset
        /// </summary>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public static NegatedCharacterSet operator ! ( CharacterSet charSet ) =>
            new NegatedCharacterSet ( charSet.CharSet );

        /// <summary>
        /// Tranforms this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[{String.Join ( "", this.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) )}]";
    }
}
