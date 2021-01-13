using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A unicode category terminal.
    /// </summary>
    public sealed class UnicodeCategoryTerminal : GrammarNode<Char>, IEquatable<UnicodeCategoryTerminal?>
    {
        /// <summary>
        /// The unicode category.
        /// </summary>
        public UnicodeCategory Category { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterUnicodeCategoryTerminal;

        /// <summary>
        /// Initializes a new unicode category terminal.
        /// </summary>
        /// <param name="category"></param>
        public UnicodeCategoryTerminal ( UnicodeCategory category )
        {
            this.Category = category;
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as UnicodeCategoryTerminal );

        /// <inheritdoc/>
        public Boolean Equals ( UnicodeCategoryTerminal? other ) =>
            other != null
            && this.Category == other.Category;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.Category );

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

        /// <summary>
        /// Checks whether two unicode category terminals are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( UnicodeCategoryTerminal? left, UnicodeCategoryTerminal? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two unicode category terminals are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( UnicodeCategoryTerminal? left, UnicodeCategoryTerminal? right ) =>
            !( left == right );
    }
}
