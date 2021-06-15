using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a node that matches a sequence of characters
    /// </summary>
    public sealed class StringTerminal : GrammarNode<Char>, IEquatable<StringTerminal?>
    {
        /// <summary>
        /// The string of characters this node matches
        /// </summary>
        public String Value { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterStringTerminal;

        /// <summary>
        /// Initializes this string node
        /// </summary>
        /// <param name="str"></param>
        public StringTerminal(String str)
        {
            this.Value = str;
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) => this.Equals(obj as StringTerminal);

        /// <inheritdoc/>
        public Boolean Equals(StringTerminal? other) =>
            other != null
            && StringComparer.Ordinal.Equals(this.Value, other.Value);

        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            HashCode.Combine(this.Value);

        /// <summary>
        /// The implicit conversion from strings to string nodes
        /// </summary>
        /// <param name="str"></param>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "The constructor can be used instead.")]
        public static implicit operator StringTerminal(String str) => new(str);

        /// <summary>
        /// Checks whether two string terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(StringTerminal? left, StringTerminal? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two string terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(StringTerminal? left, StringTerminal? right) =>
            !(left == right);
    }
}