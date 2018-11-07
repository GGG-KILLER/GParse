using System;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Parsing.Modules
{
    public class SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> : IInfixModule<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        public delegate ExpressionNodeT NodeFactory ( ExpressionNodeT left, Token<TokenTypeT> @operator, ExpressionNodeT right );

        public Int32 Precedence { get; }
        private readonly Boolean IsRightAssociative;
        private readonly NodeFactory Factory;

        public SingleTokenInfixOperatorModule ( Int32 precedence, Boolean isRightAssociative, NodeFactory factory )
        {
            this.Precedence         = precedence;
            this.IsRightAssociative = isRightAssociative;
            this.Factory            = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        public ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken ) =>
            this.Factory ( leftHandSide, readToken, parser.ParseExpression ( this.IsRightAssociative ? this.Precedence - 1 : this.Precedence ) );
    }
}
