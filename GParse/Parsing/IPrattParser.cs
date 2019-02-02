using System;
using GParse.Lexing;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a modular pratt expression parser
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrattParser<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// The parser's token reader instance
        /// </summary>
        ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <param name="precedence"></param>
        /// <returns></returns>
        ExpressionNodeT ParseExpression ( Int32 precedence );

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns></returns>
        ExpressionNodeT ParseExpression ( );
    }
}
