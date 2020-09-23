using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A numbered backreference.
    /// </summary>
    public sealed class NumberedBackreference : GrammarNode<Char>
    {
        /// <summary>
        /// The group number.
        /// </summary>
        public Int32 Position { get; }

        /// <summary>
        /// Initializes a new numbered backreference
        /// </summary>
        /// <param name="position"></param>
        public NumberedBackreference ( Int32 position )
        {
            if ( position <= 0 )
                throw new ArgumentOutOfRangeException ( nameof ( position ) );

            this.Position = position;
        }
    }
}
