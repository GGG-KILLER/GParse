using System;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a modular pratt expression parser.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="TExpressionNode"></typeparam>
    public interface IPrattParser<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        /// <summary>
        /// The token reader.
        /// </summary>
        ITokenReader<TTokenType> TokenReader { get; }

        /// <summary>
        /// Attempts to parse an expression with a minimum precedence of
        /// <paramref name="minPrecedence" />.
        /// </summary>
        /// <remarks>
        /// The minimum precedence is used to enforce the precedence of operators as well as
        /// associativity.
        ///
        /// The <see cref="Parselets.SingleTokenInfixOperatorParselet{TTokenType, TExpressionNode}" />
        /// uses the <paramref name="minPrecedence" /> parameter to implement associativity by passing in
        /// the associativity of the operator subtracted by one so that the operator itself is in the set
        /// of possible parselets.
        /// </remarks>
        /// <param name="minPrecedence"></param>
        /// <returns>The parsed expression if successful.</returns>
        Option<TExpressionNode> ParseExpression(int minPrecedence);

        /// <summary>
        /// Attempts to parse an expression.
        /// </summary>
        /// <returns>The parsed expression if successful.</returns>
        Option<TExpressionNode> ParseExpression();
    }
}