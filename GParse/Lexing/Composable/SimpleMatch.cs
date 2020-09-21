using System;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A simple match containing only whether the match was successful and its length if successful.
    /// </summary>
    public readonly struct SimpleMatch
    {
        /// <summary>
        /// Whether this match was successful.
        /// </summary>
        public Boolean IsMatch { get; }

        /// <summary>
        /// The length of the match.
        /// </summary>
        public Int32 Length { get; }

        /// <summary>
        /// Initializes a new simple match.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary" /></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary" /></param>
        public SimpleMatch ( Boolean isMatch, Int32 length )
        {
            this.IsMatch = isMatch;
            this.Length = length;
        }

        /// <summary>
        /// Deconstructs this simple match into its counterparts.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary" /></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary" /></param>
        public void Deconstruct ( out Boolean isMatch, out Int32 length ) =>
            (isMatch, length) = (this.IsMatch, this.Length);
    }
}