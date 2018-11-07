using System;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Parsing.Modules
{
    public class SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT> : IInfixModule<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        public delegate ExpressionNodeT NodeFactory ( ExpressionNodeT value, Token<TokenTypeT> @operator );

        public Int32 Precedence { get; }
        private readonly NodeFactory Factory;

        public SingleTokenPostfixOperatorModule ( Int32 precedence, NodeFactory factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence of the operator must be greater than 0" );

            this.Precedence = precedence;
            this.Factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        public ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken ) =>
            this.Factory ( leftHandSide, readToken );
    }
}
