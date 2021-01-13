using System;
using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a terminal
    /// </summary>
    public abstract class Terminal<T> : GrammarNode<T>, IEquatable<Terminal<T>?>
    {
        /// <summary>
        /// The value of the terminal
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new terminal
        /// </summary>
        /// <param name="value"></param>
        protected Terminal ( T value )
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as Terminal<T> );

        /// <inheritdoc/>
        public Boolean Equals ( Terminal<T>? other ) =>
            other != null
            && EqualityComparer<T>.Default.Equals ( this.Value, other.Value );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( this.Value );

        /// <summary>
        /// Checks whether two terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( Terminal<T>? left, Terminal<T>? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Terminal<T>? left, Terminal<T>? right ) =>
            !( left == right );
    }
}
