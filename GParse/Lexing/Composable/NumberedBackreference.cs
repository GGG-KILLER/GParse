using System;
using System.Collections.Generic;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A numbered backreference.
    /// </summary>
    public sealed class NumberedBackreference : GrammarNode<Char>, IEquatable<NumberedBackreference?>
    {
        /// <summary>
        /// The group number.
        /// </summary>
        public Int32 Position { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNumberedBackreference;

        /// <summary>
        /// Initializes a new numbered backreference
        /// </summary>
        /// <param name="position"></param>
        public NumberedBackreference ( Int32 position )
        {
            if ( position <= 0 )
                throw new ArgumentOutOfRangeException ( nameof ( position ) );

            this.Position = position;
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as NumberedBackreference );

        /// <inheritdoc/>
        public Boolean Equals ( NumberedBackreference? other ) =>
            other != null
            && this.Position == other.Position;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.Position );

        /// <summary>
        /// Checks whether two numbered backreferences are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NumberedBackreference? left, NumberedBackreference? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two numbered backreferences are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NumberedBackreference? left, NumberedBackreference? right ) =>
            !( left == right );
    }
}
