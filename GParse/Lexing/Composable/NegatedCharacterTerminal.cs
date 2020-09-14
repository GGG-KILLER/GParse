using System;
using System.Collections.Generic;
using System.Text;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Node that matches character that are not the provided one
    /// </summary>
    public class NegatedCharacterTerminal : Terminal<Char>
    {
        /// <summary>
        /// Initializes this chars other than node
        /// </summary>
        /// <param name="value"></param>
        public NegatedCharacterTerminal ( Char value ) : base ( value )
        {
        }

        /// <summary>
        /// Negates this chars other than node to match the char contained within
        /// </summary>
        /// <param name="charsOtherThanNode"></param>
        /// <returns></returns>
        public static CharacterTerminal operator ! ( NegatedCharacterTerminal charsOtherThanNode ) =>
            new CharacterTerminal ( charsOtherThanNode.Value );
    }
}
