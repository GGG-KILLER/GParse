using System;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that does not match an inclusive range
    /// </summary>
    public class NegatedCharacterRange : GrammarNode<Char>
    {
        /// <summary>
        /// The first char this range will match.
        /// </summary>
        public Char Start { get; }

        /// <summary>
        /// The last char this range will match.
        /// </summary>
        public Char End { get; }

        /// <summary>
        /// Initializes this exclusion list node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public NegatedCharacterRange ( Char start, Char end )
        {
            if ( start > end )
                throw new ArgumentException ( "Start must be less than or equal to end." );

            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Negates this negated character range.
        /// </summary>
        /// <param name="negatedRange"></param>
        /// <returns></returns>
        public static CharacterRange operator ! ( NegatedCharacterRange negatedRange ) =>
            new CharacterRange ( negatedRange.Start, negatedRange.End );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{CharUtils.ToReadableString ( this.Start )}-{CharUtils.ToReadableString ( this.End )}]";
    }
}
