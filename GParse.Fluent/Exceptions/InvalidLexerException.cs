using System;

namespace GParse.Fluent.Exceptions
{
    /// <summary>
    /// Thrown when an invalid lexer is executed
    /// </summary>
    public class InvalidLexerException : Exception
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="message"></param>
        public InvalidLexerException ( String message ) : base ( message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidLexerException ( String message, Exception innerException ) : base ( message, innerException )
        {
        }
    }
}
