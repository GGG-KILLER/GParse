using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface IReadOnlyLexer<TokenTypeT> where TokenTypeT : Enum
    {
        Boolean EOF { get; }

        Token<TokenTypeT> PeekToken ( );

        #region IsNextToken

        Boolean IsNextToken ( String ID );

        Boolean IsNextToken ( TokenTypeT type );

        Boolean IsNextToken ( String ID, TokenTypeT type );

        #endregion IsNextToken
    }
}
