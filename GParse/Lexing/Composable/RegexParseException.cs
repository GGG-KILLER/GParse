using System;
using GParse.Errors;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An exception raised while parsing a regex expression.
    /// </summary>
    public class RegexParseException : FatalParsingException
    {
        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public RegexParseException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public RegexParseException ( SourceRange range, String message ) : base ( range, message )
        {
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"><inheritdoc/></param>
        public RegexParseException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"></param>
        public RegexParseException ( SourceRange range, String message, Exception innerException ) : base ( range, message, innerException )
        {
        }
    }
}
