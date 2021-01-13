using System;
using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a non-terminal
    /// </summary>
    public sealed class NonTerminal<T> : GrammarNode<T>, IEquatable<NonTerminal<T>?>
    {
        /// <summary>
        /// The name of the production this references
        /// </summary>
        public String Name { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.NonTerminal;

        /// <summary>
        /// Initializes a non-terminal
        /// </summary>
        /// <param name="name"></param>
        public NonTerminal ( String name )
        {
            this.Name = name;
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as NonTerminal<T> );

        /// <inheritdoc/>
        public Boolean Equals ( NonTerminal<T>? other ) =>
            other != null
            && this.Name == other.Name;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( this.Name );

        /// <summary>
        /// Checks whether two non-terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NonTerminal<T>? left, NonTerminal<T>? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two non-terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NonTerminal<T>? left, NonTerminal<T>? right ) =>
            !( left == right );
    }
}
