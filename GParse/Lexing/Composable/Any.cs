using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a node that matches any single char.
    /// </summary>
    public sealed class Any : GrammarNode<Char>, IEquatable<Any>
    {
        internal static readonly Any Instance = new ( );

        /// <inheritdoc/>
        public Boolean Equals ( Any? other ) => other is not null;

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => obj is Any;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => 0;
    }
}
