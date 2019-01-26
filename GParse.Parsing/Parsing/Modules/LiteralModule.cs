using System;
using GParse.Common;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Parsing.Modules
{
    /// <summary>
    /// A module for single token literals
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class LiteralModule<TokenTypeT, ExpressionNodeT> : IPrefixModule<TokenTypeT, ExpressionNodeT>
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
        public LiteralModule ( NodeFactory factory )
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
