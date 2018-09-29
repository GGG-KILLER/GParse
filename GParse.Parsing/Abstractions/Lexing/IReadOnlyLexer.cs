using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface IReadOnlyLexer
    {
        Boolean EOF { get; }

        Token PeekToken ( );

        #region IsNextToken

        Boolean IsNextToken ( String ID );

        Boolean IsNextToken ( TokenType type );

        Boolean IsNextToken ( String ID, TokenType type );

        #endregion IsNextToken
    }
}
