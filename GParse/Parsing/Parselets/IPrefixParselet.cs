using System;
using System.Diagnostics.CodeAnalysis;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses a prefix expression.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public interface IPrefixParselet<TTokenType, ExpressionNodeT>
        where TTokenType : notnull
    {
        /// <summary>
        /// Attempts to parse a prefix expression. State will be restored by the caller.
        /// </summary>
        /// <param name="parser">The parser that called this parselet.</param>
        /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
        /// <param name="parsedExpression">The resulting parsed expression.</param>
        /// <returns>Whether the parsing was successful.</returns>
        Boolean TryParse (
            IPrattParser<TTokenType, ExpressionNodeT> parser,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out ExpressionNodeT parsedExpression );
    }
}
