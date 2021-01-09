using System;
using GParse;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a <see cref="IPrattParser{TTokenType, ExpressionNodeT}" /> builder.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public interface IPrattParserBuilder<TTokenType, ExpressionNodeT>
        where TTokenType : notnull
    {
        /// <summary>
        /// Registers a prefix expression parser module.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        void Register ( TTokenType tokenType, IPrefixParselet<TTokenType, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="prefixModule"></param>
        void Register ( TTokenType tokenType, String id, IPrefixParselet<TTokenType, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        void Register ( TTokenType tokenType, IInfixParselet<TTokenType, ExpressionNodeT> infixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="infixModule"></param>
        void Register ( TTokenType tokenType, String id, IInfixParselet<TTokenType, ExpressionNodeT> infixModule );

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        IPrattParser<TTokenType, ExpressionNodeT> CreateParser (
            ITokenReader<TTokenType> reader,
            DiagnosticList diagnostics );
    }
}
