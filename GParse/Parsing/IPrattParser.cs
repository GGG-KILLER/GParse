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
        /// Attempts to parse an expression
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Boolean TryParseExpression ( Int32 precedence, out ExpressionNodeT expression );

        /// <summary>
        /// Attempts to parse an expression
        /// </summary>
        /// <returns></returns>
        Boolean TryParseExpression ( out ExpressionNodeT expression );
    }
}
