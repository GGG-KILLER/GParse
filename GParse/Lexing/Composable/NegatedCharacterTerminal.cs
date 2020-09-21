using System;
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

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{CharUtils.ToReadableString ( this.Value )}]";
    }
}
