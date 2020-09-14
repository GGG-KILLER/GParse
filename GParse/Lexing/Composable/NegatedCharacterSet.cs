using System;
using System.Collections.Generic;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that matches anything that is not in this character set
    /// </summary>
    public class NegatedCharacterSet : CharacterSet
    {
        /// <summary>
        /// Initializes this negated character set
        /// </summary>
        /// <param name="charSet"></param>
        public NegatedCharacterSet ( params Char[] charSet ) : base ( charSet )
        {
        }

        /// <summary>
        /// Initializes this negated character set
        /// </summary>
        /// <param name="charSet"></param>
        public NegatedCharacterSet ( ISet<Char> charSet ) : base ( charSet )
        {
        }
    }
}