using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing.Errors
{
    /// <summary>
    /// Thrown when the <see cref="ILexer{TokenTypeT}" /> cannot
    /// find a token module compatible with the text ahead
    /// </summary>
    public class UnableToContinueLexingException : LocationBasedException
    {
        /// <summary>
        /// Initializes the <see cref="UnableToContinueLexingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="reader"></param>
        public UnableToContinueLexingException ( SourceLocation location, String message, SourceCodeReader reader ) : base ( location, message )
        {
            this.Reader = reader;
        }

        /// <summary>
        /// Initializes the <see cref="UnableToContinueLexingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="reader"></param>
        /// <param name="innerException"></param>
        public UnableToContinueLexingException ( SourceLocation location, String message, SourceCodeReader reader, Exception innerException ) : base ( location, message, innerException )
        {
            this.Reader = reader;
        }

        /// <summary>
        /// The reader that the <see cref="ILexer{TokenTypeT}" />
        /// was using
        /// </summary>
        public SourceCodeReader Reader { get; }
    }
}
