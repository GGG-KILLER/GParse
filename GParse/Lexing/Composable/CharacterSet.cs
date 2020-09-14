using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that matches a set of characters
    /// </summary>
    public class CharacterSet : GrammarNode<Char>
    {
        /// <summary>
        /// The set of characters this node matches
        /// </summary>
        public ISet<Char> CharSet { get; }

        /// <summary>
        /// Initializes this character set
        /// </summary>
        /// <param name="charSet"></param>
        public CharacterSet ( params Char[] charSet ) : this ( new HashSet<Char> ( charSet ) )
        {
        }

        /// <summary>
        /// Initializes this character set
        /// </summary>
        /// <param name="charSet"></param>
        public CharacterSet ( ISet<Char> charSet )
        {
            if ( charSet is null )
                throw new ArgumentNullException ( nameof ( charSet ) );
            if ( charSet.Count < 1 )
                throw new ArgumentException ( "The input charset must not be empty" );

            this.CharSet = charSet;
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
    }
}
