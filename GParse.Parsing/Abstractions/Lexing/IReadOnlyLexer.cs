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

        Token<TokenTypeT> PeekToken ( );

        #region IsNextToken

        Boolean IsNextToken ( String ID );

        Boolean IsNextToken ( TokenTypeT type );

        Boolean IsNextToken ( String ID, TokenTypeT type );

        #endregion IsNextToken
    }
}
