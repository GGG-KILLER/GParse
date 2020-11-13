using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A character terminal node.
    /// </summary>
    public sealed class CharacterTerminal : Terminal<Char>
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
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "The constructor can be used instead." )]
        public static implicit operator CharacterTerminal ( Char ch ) =>
            new CharacterTerminal ( ch );


        /// <summary>
        /// Negates this char terminal to match nodes that are not this char.
        /// </summary>
        /// <param name="charTerminal"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's the Negate extension method." )]
        public static NegatedCharacterTerminal? operator ! ( CharacterTerminal? charTerminal ) =>
            charTerminal is null ? null : new NegatedCharacterTerminal ( charTerminal.Value );

        /// <summary>
        /// Converts this terminal into a readable character string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            CharUtils.ToReadableString ( this.Value );
    }
}
