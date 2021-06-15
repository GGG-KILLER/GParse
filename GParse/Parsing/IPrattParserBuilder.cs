using System;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a <see cref="IPrattParser{TTokenType, TExpressionNode}" /> builder.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    public interface IPrattParserBuilder<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        /// <summary>
        /// Registers a prefix expression parser module.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        void Register(TTokenType tokenType, IPrefixParselet<TTokenType, TExpressionNode> prefixModule);

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="prefixModule"></param>
        void Register(TTokenType tokenType, string id, IPrefixParselet<TTokenType, TExpressionNode> prefixModule);

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        void Register(TTokenType tokenType, IInfixParselet<TTokenType, TExpressionNode> infixModule);

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="infixModule"></param>
        void Register(TTokenType tokenType, string id, IInfixParselet<TTokenType, TExpressionNode> infixModule);

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        IPrattParser<TTokenType, TExpressionNode> CreateParser(
            ITokenReader<TTokenType> reader,
            DiagnosticList diagnostics);
    }
}