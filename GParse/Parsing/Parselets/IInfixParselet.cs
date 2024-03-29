﻿using System;
using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses an infix operation.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public interface IInfixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        /// <summary>
        /// The precedence of the operator this module parses.
        /// </summary>
        int Precedence { get; }

        /// <summary>
        /// Attempts to parse an infix/postfix expression. State should be restored by the caller on failure.
        /// </summary>
        /// <param name="parser">The parser that called this parselet.</param>
        /// <param name="expression">The expression that was parsed on the left side of the infix.</param>
        /// <param name="diagnostics">The diagnostic list to be used when reporting new diagnostics.</param>
        /// <returns>The resulting parsed expression if parsing was succesful.</returns>
        Option<TExpressionNode> Parse(
            IPrattParser<TTokenType, TExpressionNode> parser,
            TExpressionNode expression,
            DiagnosticList diagnostics);
    }
}