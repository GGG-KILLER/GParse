using System;
using GParse;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// Defines the interface of a module that parses infix operations
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public interface IInfixParselet<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// The precedence of this module
        /// </summary>
        Int32 Precedence { get; }

        /// <summary>
        /// Parses an infix expression
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="leftHandSide"></param>
        /// <param name="readToken"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken, IProgress<Diagnostic> diagnosticEmitter );
    }
}
