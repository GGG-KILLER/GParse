using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A character terminal node.
    /// </summary>
    public class CharacterTerminal : Terminal<Char>
    {
        /// <summary>
        /// Initializes this character terminal node.
        /// </summary>
        /// <param name="value"></param>
        public CharacterTerminal ( Char value ) : base ( value )
        {
        }

        /// <summary>
        /// Converts a char to a char terminal.
        /// </summary>
        /// <param name="ch"></param>
        public static implicit operator CharacterTerminal ( Char ch ) =>
            new CharacterTerminal ( ch );

        /// <summary>
        /// Negates this char terminal to match nodes that are not this char.
        /// </summary>
        /// <param name="charTerminal"></param>
        /// <returns></returns>
        public static NegatedCharacterTerminal operator ! ( CharacterTerminal charTerminal ) =>
            new NegatedCharacterTerminal ( charTerminal.Value );

        /// <summary>
        /// Converts this terminal into a readable character string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            CharUtils.ToReadableString ( this.Value );
    }
}
