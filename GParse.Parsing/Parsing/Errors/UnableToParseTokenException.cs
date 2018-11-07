using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;

namespace GParse.Parsing.Parsing.Errors
{
    public class UnableToParseTokenException<TokenTypeT> : ParsingException where TokenTypeT : Enum
    {
        public readonly Token<TokenTypeT> Token;

        public UnableToParseTokenException ( SourceLocation location, Token<TokenTypeT> token, String message ) : base ( location, message )
        {
            this.Token = token;
        }

        public UnableToParseTokenException ( SourceLocation location, Token<TokenTypeT> token, String message, Exception innerException ) : base ( location, message, innerException )
        {
            this.Token = token;
        }
    }
}
