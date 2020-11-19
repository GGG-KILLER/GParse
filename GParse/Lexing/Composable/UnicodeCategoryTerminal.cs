using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A unicode category terminal.
    /// </summary>
    public sealed class UnicodeCategoryTerminal : GrammarNode<Char>
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
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate method can be used instead." )]
        public static NegatedUnicodeCategoryTerminal operator ! ( UnicodeCategoryTerminal categoryTerminal )
        {
            if ( categoryTerminal is null )
                throw new ArgumentNullException ( nameof ( categoryTerminal ) );
            return new NegatedUnicodeCategoryTerminal ( categoryTerminal.Category );
        }
    }
}
