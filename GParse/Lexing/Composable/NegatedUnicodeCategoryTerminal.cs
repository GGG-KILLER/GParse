using System;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negated unicode category terminal.
    /// </summary>
    public sealed class NegatedUnicodeCategoryTerminal : GrammarNode<Char>
    {
        /// <summary>
        /// The unicode category.
        /// </summary>
        public UnicodeCategory Category { get; }

        /// <summary>
        /// Initializes a new negated unicode category terminal.
        /// </summary>
        /// <param name="category"></param>
        public NegatedUnicodeCategoryTerminal ( UnicodeCategory category )
        {
            this.Category = category;
        }

        /// <summary>
        /// Negated a negated unicode category terminal.
        /// </summary>
        /// <param name="negatedCategoryTerminal"></param>
        /// <returns></returns>
        public static UnicodeCategoryTerminal operator ! ( NegatedUnicodeCategoryTerminal negatedCategoryTerminal ) =>
            new UnicodeCategoryTerminal ( negatedCategoryTerminal.Category );
    }
}
