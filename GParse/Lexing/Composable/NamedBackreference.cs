using System;
using System.Collections.Generic;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a backreference to a captured group.
    /// </summary>
    public sealed class NamedBackreference : GrammarNode<Char>, IEquatable<NamedBackreference?>
    {
        /// <summary>
        /// The name of the capture this reference references.
        /// </summary>
        public String Name { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNamedBackreference;

        /// <summary>
        /// Initializes a new backreference.
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="/summary" /></param>
        public NamedBackreference(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            this.Name = name;
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) => this.Equals(obj as NamedBackreference);

        /// <inheritdoc/>
        public Boolean Equals(NamedBackreference? other) => other != null && this.Name == other.Name;

        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(this.Name);

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString() =>
            $"\\k<{this.Name}>";

        /// <summary>
        /// Checks whether two named backreferences are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(NamedBackreference? left, NamedBackreference? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two named backreferences are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(NamedBackreference? left, NamedBackreference? right) => !(left == right);
    }
}