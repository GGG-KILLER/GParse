using System;
using GParse;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Defines the interface of a <see cref="IPrattParser{TokenTypeT, ExpressionNodeT}" /> builder.
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public interface IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Registers a prefix expression parser module.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, String id, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, String id, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser (
            ITokenReader<TokenTypeT> reader,
            DiagnosticList diagnostics );
    }
}
