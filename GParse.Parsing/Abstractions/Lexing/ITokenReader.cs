using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ITokenReader<TokenTypeT> where TokenTypeT : Enum
    {
        SourceLocation Location { get; }

        Token<TokenTypeT> Lookahead ( Int32 offset = 0 );

        Token<TokenTypeT> Consume ( );

        #region Accept

        Boolean Accept ( String ID, out Token<TokenTypeT> token );

        Boolean Accept ( String ID );

        Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token );

        Boolean Accept ( TokenTypeT type );

        Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token );

        Boolean Accept ( String ID, TokenTypeT type );

        #endregion Accept

        #region Expect

        Token<TokenTypeT> Expect ( String ID );

        Token<TokenTypeT> Expect ( TokenTypeT type );

        Token<TokenTypeT> Expect ( String ID, TokenTypeT type );

        #endregion Expect
    }
}
