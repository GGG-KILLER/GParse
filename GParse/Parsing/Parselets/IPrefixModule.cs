using System;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses a prefix expression
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IPrefixParselet<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// Parses a prefix expression
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="readToken"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        ExpressionNodeT ParsePrefix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, Token<TokenTypeT> readToken, IProgress<Diagnostic> diagnosticEmitter );
    }
}
