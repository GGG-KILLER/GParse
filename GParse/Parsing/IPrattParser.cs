using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

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
        /// <param name="expression"></param>
        /// <returns></returns>
        Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out TExpressionNode expression );

        /// <summary>
        /// Attempts to parse an expression.
        /// </summary>
        /// <param name="expression">The parsed expression if successful.</param>
        /// <returns>Whether the parsing happened without any fatal errors.</returns>
        Boolean TryParseExpression ( [NotNullWhen ( true )] out TExpressionNode expression );
    }
}
