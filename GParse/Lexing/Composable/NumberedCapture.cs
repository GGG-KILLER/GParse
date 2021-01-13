using System;
using System.Collections.Generic;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a numbered capture group.
    /// </summary>
    public sealed class NumberedCapture : GrammarNodeContainer<Char>, IEquatable<NumberedCapture?>
    {
        /// <summary>
        /// The capture's position.
        /// </summary>
        public Int32 Position { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNumberedCapture;

        /// <summary>
        /// Initializes a new numbered capture group.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="node"></param>
        public NumberedCapture ( Int32 position, GrammarNode<Char> node ) : base ( node )
        {
            this.Position = position;
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as NumberedCapture );

        /// <inheritdoc/>
        public Boolean Equals ( NumberedCapture? other ) =>
            other != null
            && this.Position == other.Position
            && EqualityComparer<GrammarNode<Char>>.Default.Equals ( this.InnerNode, other.InnerNode );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.InnerNode, this.Position );

        /// <summary>
        /// Checks whether two numbered captures are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NumberedCapture? left, NumberedCapture? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two numbered captures are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NumberedCapture? left, NumberedCapture? right ) =>
            !( left == right );
    }
}
