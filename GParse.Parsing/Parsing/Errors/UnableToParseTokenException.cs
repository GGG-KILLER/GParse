using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;

namespace GParse.Parsing.Parsing.Errors
{
    /// <summary>
    /// Thrown when a parser cannot continue its work because of
    /// an unknown token
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class UnableToParseTokenException<TokenTypeT> : ParsingException where TokenTypeT : Enum
    {
        /// <summary>
        /// The token that was unable to be parsed
        /// </summary>
        public readonly Token<TokenTypeT> Token;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public UnableToParseTokenException ( SourceLocation location, Token<TokenTypeT> token, String message ) : base ( location, message )
        {
            this.Token = token;
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnableToParseTokenException ( SourceLocation location, Token<TokenTypeT> token, String message, Exception innerException ) : base ( location, message, innerException )
        {
            this.Token = token;
        }
    }
}
