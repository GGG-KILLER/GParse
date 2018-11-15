using System;

namespace GParse.Common.Errors
{
    /// <summary>
    /// Thrown when an unexpected char is found
    /// </summary>
    public class UnexpectedCharException : LocationBasedException
    {
        /// <summary>
        /// Unexpected char found
        /// </summary>
        public readonly Char UnexpectedChar;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="ch"></param>
        public UnexpectedCharException ( SourceLocation location, Char ch ) : base ( location, $"Unexpected character: '{ch}'" )
        {
            this.UnexpectedChar = ch;
        }
    }
}
