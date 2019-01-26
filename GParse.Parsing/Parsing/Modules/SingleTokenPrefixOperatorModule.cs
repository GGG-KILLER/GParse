using System;
using GParse.Common;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Parsing.Modules
{
    /// <summary>
    /// A module for single-token prefix operators
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT> : IPrefixModule<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// Defines the interface for a node factory
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate ExpressionNodeT NodeFactory ( Token<TokenTypeT> @operator, ExpressionNodeT value );

        private readonly Int32 Precedence;
        private readonly NodeFactory Factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPrefixOperatorModule ( Int32 precedence, NodeFactory factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence must be a value greater than 0." );

            this.Precedence = precedence;
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
            this.Factory ( readToken, parser.ParseExpression ( this.Precedence ) );
    }
}
