using System;

namespace GParse.Common.Errors
{
    /// <summary>
    /// Exception thrown when an error happens while parsing
    /// </summary>
    public class ParsingException : LocationBasedException
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public ParsingException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ParsingException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
