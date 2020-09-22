using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that matches anything that is not in this character set
    /// </summary>
    public class NegatedCharacterSet : GrammarNode<Char>
    {
        /// <summary>
        /// The character set.
        /// </summary>
        public IImmutableSet<Char> CharSet { get; }

        /// <summary>
        /// Initializes this negated character set
        /// </summary>
        /// <param name="charSet"></param>
        public NegatedCharacterSet ( params Char[] charSet ) : this ( ( IEnumerable<Char> ) charSet )
        {
        }

        /// <summary>
        /// Initializes this negated character set
        /// </summary>
        /// <param name="charSet"></param>
        public NegatedCharacterSet ( IEnumerable<Char> charSet )
        {
            this.CharSet = charSet.ToImmutableHashSet ( );
        }

        /// <summary>
        /// Negates a negated character set.
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <returns></returns>
        public static CharacterSet operator ! ( NegatedCharacterSet negatedCharacterSet ) =>
            new CharacterSet ( negatedCharacterSet.CharSet );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{String.Join ( "", this.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) )}]";
    }
}