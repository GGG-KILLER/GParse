using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexer : IReadOnlyLexer
    {
        Token ConsumeToken ( );

        #region AcceptToken

        Boolean AcceptToken ( String ID );

        Boolean AcceptToken ( TokenType type );

        Boolean AcceptToken ( String ID, TokenType type );

        #endregion AcceptToken
    }
}
