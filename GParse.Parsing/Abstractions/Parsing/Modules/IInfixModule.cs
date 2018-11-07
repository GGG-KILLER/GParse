using System;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Abstractions.Parsing.Modules
{
    public interface IInfixModule<TokenTypeT, ExpressionNodeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// The precedence of this module
        /// </summary>
        Int32 Precedence { get; }

        /// <summary>
        /// Parses an infix expression
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="leftHandSide"></param>
        /// <param name="readToken"></param>
        /// <returns></returns>
        ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken );
    }
}
