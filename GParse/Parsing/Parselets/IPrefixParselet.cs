using GParse.Lexing;
using Tsu;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses a prefix expression.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public interface IPrefixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        /// <summary>
        /// Attempts to parse a prefix expression. State will be restored by the caller.
        /// </summary>
        /// <param name="parser">The parser that called this parselet.</param>
        /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
        /// <returns>The resulting parsed expression if successful.</returns>
        Option<TExpressionNode> Parse(
            IPrattParser<TTokenType, TExpressionNode> parser,
            DiagnosticList diagnostics);
    }
}