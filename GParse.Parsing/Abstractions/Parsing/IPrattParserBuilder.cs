using System;
using GParse.Common;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Abstractions.Parsing
{
    /// <summary>
    /// Defines the interface of a
    /// <see cref="IPrattParser{TokenTypeT, ExpressionNodeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader, IProgress<Diagnostic> diagnosticEmitter );
    }
}
