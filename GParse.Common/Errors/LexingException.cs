using System;

namespace GParse.Common.Errors
{
    /// <summary>
    /// Exception thrown when an error happens while lexing
    /// </summary>
    public class LexingException : LocationBasedException
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="location"></param>
        public LexingException ( String message, SourceLocation location ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public LexingException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public LexingException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
