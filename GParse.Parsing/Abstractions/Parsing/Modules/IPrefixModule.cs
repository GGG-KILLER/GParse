using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Parsing.Modules
{
    /// <summary>
    /// Defines the interface of a module that parses a prefix expression
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrefixModule<TokenTypeT, ExpressionNodeT>
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
