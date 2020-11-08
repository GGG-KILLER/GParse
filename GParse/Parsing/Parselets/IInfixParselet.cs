using System;
using System.Diagnostics.CodeAnalysis;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses an infix operation.
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public interface IInfixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The precedence of the operator this module parses.
        /// </summary>
        Int32 Precedence { get; }

        /// <summary>
        /// Attempts to parse an infix/postfix expression. State should be restored by the caller on failure.
        /// </summary>
        /// <param name="parser">The parser that called this parselet.</param>
        /// <param name="expression">The expression that was parsed on the left side of the infix.</param>
        /// <param name="diagnostics">The diagnostic list to be used when reporting new diagnostics.</param>
        /// <param name="parsedExpression">The resulting parsed expression.</param>
        /// <returns>Whether the parsing was succesful.</returns>
        Boolean TryParse (
            IPrattParser<TokenTypeT, ExpressionNodeT> parser,
            ExpressionNodeT expression,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out ExpressionNodeT parsedExpression );
    }
}
