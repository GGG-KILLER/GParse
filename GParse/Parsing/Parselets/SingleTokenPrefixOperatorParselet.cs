using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a prefix expression node.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    /// <param name="operator">The operator token.</param>
    /// <param name="operand">The operand expression.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <returns>The resulting parsed expression if successful.</returns>
    public delegate Option<TExpressionNode> PrefixNodeFactory<TTokenType, TExpressionNode>(
        Token<TTokenType> @operator,
        TExpressionNode operand,
        DiagnosticList diagnostics)
        where TTokenType : notnull;

    /// <summary>
    /// A module for single-token prefix operators
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="TExpressionNode"></typeparam>
    public class SingleTokenPrefixOperatorParselet<TTokenType, TExpressionNode> : IPrefixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        private readonly int _precedence;
        private readonly PrefixNodeFactory<TTokenType, TExpressionNode> _factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPrefixOperatorParselet(int precedence, PrefixNodeFactory<TTokenType, TExpressionNode> factory)
        {
            if (precedence < 1)
                throw new ArgumentOutOfRangeException(nameof(precedence), "Precedence must be a value greater than 0.");

            _precedence = precedence;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc />
        public Option<TExpressionNode> Parse(
            IPrattParser<TTokenType, TExpressionNode> parser,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var prefix = parser.TokenReader.Consume();
            return parser.ParseExpression(_precedence)
                         .AndThen(expression => _factory(prefix, expression, diagnostics));
        }
    }
}