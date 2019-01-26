using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Fluent.Exceptions
{
    /// <summary>
    /// Thrown when a invalid matcher expression is given
    /// </summary>
    public class InvalidExpressionException : FatalParsingException
    {
        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public InvalidExpressionException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidExpressionException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
