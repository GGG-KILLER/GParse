using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Node that matches character that are not the provided one
    /// </summary>
    public sealed class NegatedCharacterTerminal : Terminal<Char>, IEquatable<NegatedCharacterTerminal?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNegatedTerminal;

        /// <summary>
        /// Initializes this chars other than node
        /// </summary>
        /// <param name="value"></param>
        public NegatedCharacterTerminal ( Char value ) : base ( value )
        {
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as NegatedCharacterTerminal );

        /// <inheritdoc/>
        public Boolean Equals ( NegatedCharacterTerminal? other ) =>
            other != null
            && base.Equals ( other ) && this.Value == other.Value;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( base.GetHashCode ( ), this.Value );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{CharUtils.ToReadableString ( this.Value )}]";


        /// <summary>
        /// Negates this chars other than node to match the char contained within
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        public static CharacterTerminal operator ! ( NegatedCharacterTerminal negatedCharacterTerminal )
        {
            if ( negatedCharacterTerminal is null )
                throw new ArgumentNullException ( nameof ( negatedCharacterTerminal ) );
            return new CharacterTerminal ( negatedCharacterTerminal.Value );
        }

        /// <summary>
        /// Checks whether two negated character terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NegatedCharacterTerminal? left, NegatedCharacterTerminal? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two negated character terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NegatedCharacterTerminal? left, NegatedCharacterTerminal? right ) =>
            !( left == right );
    }
}
