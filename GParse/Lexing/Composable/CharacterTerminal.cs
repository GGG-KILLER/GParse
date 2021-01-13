using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A character terminal node.
    /// </summary>
    public sealed class CharacterTerminal : Terminal<Char>, IEquatable<CharacterTerminal?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterTerminal;

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
            new ( ch );


        /// <summary>
        /// Negates this char terminal to match nodes that are not this char.
        /// </summary>
        /// <param name="charTerminal"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's the Negate extension method." )]
        public static NegatedCharacterTerminal operator ! ( CharacterTerminal charTerminal )
        {
            if ( charTerminal is null )
                throw new ArgumentNullException ( nameof ( charTerminal ) );
            return new NegatedCharacterTerminal ( charTerminal.Value );
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as CharacterTerminal );

        /// <inheritdoc/>
        public Boolean Equals ( CharacterTerminal? other ) =>
            other != null
            && base.Equals ( other )
            && this.Value == other.Value;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( base.GetHashCode ( ), this.Value );

        /// <summary>
        /// Converts this terminal into a readable character string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            CharUtils.ToReadableString ( this.Value );

        /// <summary>
        /// Checks whether two character terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( CharacterTerminal? left, CharacterTerminal? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two character terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( CharacterTerminal? left, CharacterTerminal? right ) =>
            !( left == right );
    }
}
