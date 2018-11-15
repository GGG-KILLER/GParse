using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Fluent.Exceptions
{
    /// <summary>
    /// Thrown when the parser has an error it cannot recover from
    /// </summary>
    public class FatalParseException : ParsingException
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public FatalParseException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParseException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
