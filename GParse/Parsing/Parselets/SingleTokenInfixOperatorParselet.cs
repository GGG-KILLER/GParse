using System;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a infix expression node
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    /// <param name="left">The expression on the left side of the operator.</param>
    /// <param name="op">The operator token.</param>
    /// <param name="right">The expression on the right side of the operator.</param>
    /// <returns>The resulting parsed expression if it was successful.</returns>
    public delegate Option<TExpressionNode> InfixNodeFactory<TTokenType, TExpressionNode>(
        TExpressionNode left,
        Token<TTokenType> op,
        TExpressionNode right)
        where TTokenType : notnull;

    /// <summary>
    /// A module that can parse an infix operation with an operator composed of a single token
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public class SingleTokenInfixOperatorParselet<TTokenType, TExpressionNode> : IInfixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        private readonly bool _isRightAssociative;
        private readonly InfixNodeFactory<TTokenType, TExpressionNode> _factory;

        /// <inheritdoc />
        public int Precedence { get; }

        /// <summary>
        /// Initializes a new single token infix operator parselet.
        /// </summary>
        /// <param name="precedence">The operator's precedence.</param>
        /// <param name="isRightAssociative">Whether the operator is right-associative.</param>
        /// <param name="factory">The method that transforms the infix operation into an expression node.</param>
        public SingleTokenInfixOperatorParselet(
            int precedence,
            bool isRightAssociative,
            InfixNodeFactory<TTokenType, TExpressionNode> factory)
        {
            Precedence = precedence;
            _isRightAssociative = isRightAssociative;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc />
        public Option<TExpressionNode> Parse(
            IPrattParser<TTokenType, TExpressionNode> parser,
            TExpressionNode expression,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var op = parser.TokenReader.Consume();

            // We decrease the precedence by one on right-associative operators because the minimum
            // precedence passed to TryParseExpression is exclusive (meaning that the precedence of the
            // infix parselets must be higher than the one we pass it.
            // TODO: Check if this cannot create bugs with other operators that have the same precedence.
            int minPrecedence;
            if (_isRightAssociative)
                minPrecedence = Precedence - 1;
            else
                minPrecedence = Precedence;

            return parser.ParseExpression(minPrecedence)
                         .AndThen(nextExpr => _factory(expression, op, nextExpr));
        }
    }
}