using System;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a grammar node that does not match an inclusive range
    /// </summary>
    public class NegatedCharacterRange : CharacterRange
    {
        /// <summary>
        /// Initializes this exclusion list node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public NegatedCharacterRange ( Char start, Char end ) : base ( start, end )
        {
        }
    }
}
