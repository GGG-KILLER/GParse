using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Node that matches character that are not the provided one
    /// </summary>
    public sealed class NegatedCharacterTerminal : Terminal<Char>
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
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        public static CharacterTerminal? operator ! ( NegatedCharacterTerminal? negatedCharacterTerminal ) =>
            negatedCharacterTerminal is null ? null : new CharacterTerminal ( negatedCharacterTerminal.Value );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{CharUtils.ToReadableString ( this.Value )}]";
    }
}
