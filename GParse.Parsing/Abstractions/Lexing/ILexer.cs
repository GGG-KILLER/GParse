using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexer<TokenTypeT> : IReadOnlyLexer<TokenTypeT> where TokenTypeT : Enum
    {
        Token<TokenTypeT> Consume ( );

        #region AcceptToken

        Boolean Accept ( String ID, out Token<TokenTypeT> token );

        Boolean Accept ( String ID );

        Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token );

        Boolean Accept ( TokenTypeT type );

        Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token );

        Boolean Accept ( String ID, TokenTypeT type );

        #endregion AcceptToken
    }
}
