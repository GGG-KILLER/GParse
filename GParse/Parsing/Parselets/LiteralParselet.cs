using System;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A module for single token literals
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class LiteralParselet<TokenTypeT, ExpressionNodeT> : IPrefixParselet<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// Defines the interface of a node factory
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public delegate ExpressionNodeT NodeFactory ( Token<TokenTypeT> token );

        private readonly NodeFactory Factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="factory"></param>
        public LiteralParselet ( NodeFactory factory )
        {
            this.Factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="readToken"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public ExpressionNodeT ParsePrefix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, Token<TokenTypeT> readToken, IProgress<Diagnostic> diagnosticEmitter ) =>
            this.Factory ( readToken );
    }
}
