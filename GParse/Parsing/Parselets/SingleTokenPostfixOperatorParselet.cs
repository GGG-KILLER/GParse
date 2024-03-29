﻿using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a postfix expression node.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    /// <param name="operand">The operand expression.</param>
    /// <param name="operator">The operator token.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <returns>The parsed expression if successful.</returns>
    public delegate Option<TExpressionNode> PostfixNodeFactory<TTokenType, TExpressionNode>(
        TExpressionNode operand,
        Token<TTokenType> @operator,
        DiagnosticList diagnostics)
        where TTokenType : notnull;

    /// <summary>
    /// A module that can parse a postfix operation with an
    /// operator that is composed of a single token.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public class SingleTokenPostfixOperatorParselet<TTokenType, TExpressionNode> : IInfixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        private readonly PostfixNodeFactory<TTokenType, TExpressionNode> _factory;

        /// <inheritdoc />
        public int Precedence { get; }

        /// <summary>
        /// Initializes a new single token postfix operator parselet.
        /// </summary>
        /// <param name="precedence">The precedence of the operator this parselet is parsing.</param>
        /// <param name="factory">The method responsible for creating the postfix expression node.</param>
        public SingleTokenPostfixOperatorParselet(int precedence, PostfixNodeFactory<TTokenType, TExpressionNode> factory)
        {
            if (precedence < 1)
                throw new ArgumentOutOfRangeException(nameof(precedence), "Precedence of the operator must be greater than 0");

            Precedence = precedence;
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

            return _factory(expression, parser.TokenReader.Consume(), diagnostics);
        }
    }
}