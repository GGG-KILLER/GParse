using System;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Parsing.Modules
{
    /// <summary>
    /// A module that can parse an infix operation with an
    /// operator composed of a single token
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> : IInfixModule<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        /// <summary>
        /// Defines the interface of a token factory
        /// </summary>
        /// <param name="left"></param>
        /// <param name="operator"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public delegate ExpressionNodeT NodeFactory ( ExpressionNodeT left, Token<TokenTypeT> @operator, ExpressionNodeT right );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Int32 Precedence { get; }

        private readonly Boolean IsRightAssociative;
        private readonly NodeFactory Factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public SingleTokenInfixOperatorModule ( Int32 precedence, Boolean isRightAssociative, NodeFactory factory )
        {
            this.Precedence = precedence;
            this.IsRightAssociative = isRightAssociative;
            this.Factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="leftHandSide"></param>
        /// <param name="readToken"></param>
        /// <returns></returns>
        public ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken ) =>
            this.Factory ( leftHandSide, readToken, parser.ParseExpression ( this.IsRightAssociative ? this.Precedence - 1 : this.Precedence ) );
    }
}
