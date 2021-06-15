using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negated unicode category terminal.
    /// </summary>
    public sealed class NegatedUnicodeCategoryTerminal : GrammarNode<Char>, IEquatable<NegatedUnicodeCategoryTerminal?>
    {
        /// <summary>
        /// The unicode category.
        /// </summary>
        public UnicodeCategory Category { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNegatedUnicodeCategoryTerminal;

        /// <summary>
        /// Initializes a new negated unicode category terminal.
        /// </summary>
        /// <param name="category"></param>
        public NegatedUnicodeCategoryTerminal(UnicodeCategory category)
        {
            this.Category = category;
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            this.Equals(obj as NegatedUnicodeCategoryTerminal);

        /// <inheritdoc/>
        public Boolean Equals(NegatedUnicodeCategoryTerminal? other) =>
            other != null
            && this.Category == other.Category;

        /// <inheritdoc/>
        public override Int32 GetHashCode() =>
            HashCode.Combine(this.Category);

        /// <summary>
        /// Negates a negated unicode category terminal.
        /// </summary>
        /// <param name="negatedCategoryTerminal"></param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead.")]
        public static UnicodeCategoryTerminal operator !(NegatedUnicodeCategoryTerminal negatedCategoryTerminal)
        {
            if (negatedCategoryTerminal is null)
                throw new ArgumentNullException(nameof(negatedCategoryTerminal));
            return new UnicodeCategoryTerminal(negatedCategoryTerminal.Category);
        }

        /// <summary>
        /// Checks whether two negated unicode category terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(NegatedUnicodeCategoryTerminal? left, NegatedUnicodeCategoryTerminal? right)
        {
            if (right is null) return left is null;
            return right.Equals(left);
        }

        /// <summary>
        /// Checks whether two negated unicode category terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(NegatedUnicodeCategoryTerminal? left, NegatedUnicodeCategoryTerminal? right) =>
            !(left == right);
    }
}