using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that does not match an inclusive range
    /// </summary>
    public sealed class NegatedCharacterRange : GrammarNode<Char>
    {
        /// <summary>
        /// The character range not matched by this node.
        /// </summary>
        public Range<Char> Range { get; }

        /// <summary>
        /// Initializes a new negated character range grammar node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public NegatedCharacterRange ( Char start, Char end ) : this ( new Range<Char> ( start, end ) )
        {
        }

        /// <summary>
        /// Initializes a new negated character range grammar node.
        /// </summary>
        /// <param name="range"></param>
        public NegatedCharacterRange ( Range<Char> range )
        {
            this.Range = range;
        }

        /// <summary>
        /// Negates this negated character range.
        /// </summary>
        /// <param name="negatedRange"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Negate extension method can be used instead." )]
        public static CharacterRange? operator ! ( NegatedCharacterRange? negatedRange ) =>
            negatedRange is null ? null : new CharacterRange ( negatedRange.Range );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{CharUtils.ToReadableString ( this.Range.Start )}-{CharUtils.ToReadableString ( this.Range.End )}]";
    }
}
