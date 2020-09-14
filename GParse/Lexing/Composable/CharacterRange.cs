using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that matches an inclusive range
    /// </summary>
    public class CharacterRange : GrammarNode<Char>
    {
        /// <summary>
        /// The first char this range will match
        /// </summary>
        public Char Start { get; }

        /// <summary>
        /// The last char this range will match
        /// </summary>
        public Char End { get; }

        /// <summary>
        /// Initializes this character range grammar node
        /// </summary>
        /// <param name="start">The first char this range will match</param>
        /// <param name="end">The last char this range will match</param>
        public CharacterRange ( Char start, Char end )
        {
            if ( start > end )
                throw new ArgumentException ( "Start must be less than or equal to end." );

            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// The implicit conversion operator from a range tuple to a char range node
        /// </summary>
        /// <param name="range"></param>
        public static implicit operator CharacterRange ( (Char start, Char end) range ) =>
            new CharacterRange ( range.start, range.end );
    }
}