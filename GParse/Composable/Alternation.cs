using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Composable
{
    /// <summary>
    /// Represents an alternation of different possible grammar trees.
    /// </summary>
    public sealed class Alternation<T> : GrammarNodeListContainer<Alternation<T>, T>, IEquatable<Alternation<T>?>
    {
        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.Alternation;

        /// <summary>
        /// Initializes an alternation.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Alternation(params GrammarNode<T>[] grammarNodes) : base(grammarNodes, true)
        {
        }

        /// <summary>
        /// Initializes an alternation.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Alternation(IEnumerable<GrammarNode<T>> grammarNodes) : base(grammarNodes, true)
        {

        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) => this.Equals(obj as Alternation<T>);

        /// <inheritdoc/>
        public Boolean Equals(Alternation<T>? other) =>
            other != null
            && this.GrammarNodes.SequenceEqual(other.GrammarNodes);

        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            var hash = new HashCode();
            foreach (GrammarNode<T> node in this.GrammarNodes) hash.Add(node);
            return hash.ToHashCode();
        }

        /// <summary>
        /// Checks whether two alternations are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(Alternation<T>? left, Alternation<T>? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two alternations are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(Alternation<T>? left, Alternation<T>? right) =>
            !(left == right);
    }
}