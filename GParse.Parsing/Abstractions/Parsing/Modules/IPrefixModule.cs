using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Abstractions.Parsing.Modules
{
    public interface IPrefixModule<TokenTypeT, ExpressionNodeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// Parses a prefix expression
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="readToken"></param>
        /// <returns></returns>
        ExpressionNodeT ParsePrefix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, Token<TokenTypeT> readToken );
    }
}
