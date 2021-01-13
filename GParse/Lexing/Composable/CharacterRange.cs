using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that matches an inclusive range.
    /// </summary>
    public sealed class CharacterRange : GrammarNode<Char>, IEquatable<CharacterRange?>
    {
        /// <summary>
        /// The character range matched by this node.
        /// </summary>
        public Range<Char> Range { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterRange;

        /// <summary>
        /// Initializes this character range grammar node.
        /// </summary>
        /// <param name="start">The range's start.</param>
        /// <param name="end">The range's end.</param>
        public CharacterRange ( Char start, Char end ) : this ( new Range<Char> ( start, end ) )
        {
        }

        /// <summary>
        /// Initializes a new character range grammar node.
        /// </summary>
        /// <param name="range"></param>
        public CharacterRange ( Range<Char> range )
        {
            this.Range = range;
        }

#if HAS_VALUETUPLE

        /// <summary>
        /// The implicit conversion operator from a range tuple to a char range node
        /// </summary>
        /// <param name="range"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "The constructor can be used instead." )]
        public static implicit operator CharacterRange ( (Char start, Char end) range ) => new ( range.start, range.end );

#endif // HAS_VALUETUPLE

        /// <summary>
        /// Converts a range into this node.
        /// </summary>
        /// <param name="range"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "The constructor can be used instead." )]
        public static implicit operator CharacterRange ( Range<Char> range ) => new ( range );


        /// <summary>
        /// Negates a character range.
        /// </summary>
        /// <param name="characterRange">The range to be negated.</param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "The Negate extension method can be used instead." )]
        public static NegatedCharacterRange operator ! ( CharacterRange characterRange )
        {
            if ( characterRange is null )
                throw new ArgumentNullException ( nameof ( characterRange ) );
            return new NegatedCharacterRange ( characterRange.Range );
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as CharacterRange );

        /// <inheritdoc/>
        public Boolean Equals ( CharacterRange? other ) =>
            other != null && this.Range.Equals ( other.Range );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( this.Range );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[{CharUtils.ToReadableString ( this.Range.Start )}-{CharUtils.ToReadableString ( this.Range.End )}]";

        /// <summary>
        /// Checks whether two character ranges are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( CharacterRange? left, CharacterRange? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two character ranges are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( CharacterRange? left, CharacterRange? right ) =>
            !( left == right );
    }
}