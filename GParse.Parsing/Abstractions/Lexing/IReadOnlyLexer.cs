using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface IReadOnlyLexer<TokenTypeT> where TokenTypeT : IEquatable<TokenTypeT>
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
