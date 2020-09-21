using System;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A unicode category terminal.
    /// </summary>
    public class UnicodeCategoryTerminal : GrammarNode<Char>
    {
        /// <summary>
        /// The unicode category.
        /// </summary>
        public UnicodeCategory Category { get; }

        /// <summary>
        /// Initializes a new unicode category terminal.
        /// </summary>
        /// <param name="category"></param>
        public UnicodeCategoryTerminal ( UnicodeCategory category )
        {
            this.Category = category;
        }

        /// <summary>
        /// Negates a unicode category terminal.
        /// </summary>
        /// <param name="categoryTerminal"></param>
        /// <returns></returns>
        public static NegatedUnicodeCategoryTerminal operator ! ( UnicodeCategoryTerminal categoryTerminal ) =>
            new NegatedUnicodeCategoryTerminal ( categoryTerminal.Category );
    }
}
