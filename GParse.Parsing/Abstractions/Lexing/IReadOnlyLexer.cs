using System;
using GParse.Common;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface IReadOnlyLexer<TokenTypeT> where TokenTypeT : Enum
    {
        // Let user have access to reader maybe(?)

        SourceLocation Location { get; }

        Int32 ContentLeft { get; }

        Boolean EOF { get; }

        Token<TokenTypeT> Peek ( );

        #region IsNextToken

        Boolean IsNext ( String ID, out Token<TokenTypeT> token );

        Boolean IsNext ( String ID );

        Boolean IsNext ( TokenTypeT type, out Token<TokenTypeT> token );

        Boolean IsNext ( TokenTypeT type );

        Boolean IsNext ( String ID, TokenTypeT type, out Token<TokenTypeT> token );

        Boolean IsNext ( String ID, TokenTypeT type );

        #endregion IsNextToken
    }
}
