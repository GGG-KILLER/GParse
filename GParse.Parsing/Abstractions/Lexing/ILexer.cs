﻿using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexer<TokenTypeT> : IReadOnlyLexer<TokenTypeT> where TokenTypeT : Enum
    {
        Token<TokenTypeT> ConsumeToken ( );

        #region AcceptToken

        Boolean AcceptToken ( String ID );

        Boolean AcceptToken ( TokenTypeT type );

        Boolean AcceptToken ( String ID, TokenTypeT type );

        #endregion AcceptToken
    }
}
