using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a literal node expression
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    /// <param name="token">The obtained token.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <returns>The resulting expression node if it was succesful.</returns>
    public delegate Option<TExpressionNode> LiteralNodeFactory<TTokenType, TExpressionNode>(
        Token<TTokenType> token,
        DiagnosticList diagnostics)
        where TTokenType : notnull;

    /// <summary>
    /// A module for single token literals
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public class LiteralParselet<TTokenType, TExpressionNode> : IPrefixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        private readonly LiteralNodeFactory<TTokenType, TExpressionNode> _factory;

        /// <summary>
        /// Initializes a new literal parselet.
        /// </summary>
        /// <param name="factory">The function that transforms a token into an expression node.</param>
        public LiteralParselet(LiteralNodeFactory<TTokenType, TExpressionNode> factory)
        {
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

            return _factory(parser.TokenReader.Consume(), diagnostics);
        }
    }
}