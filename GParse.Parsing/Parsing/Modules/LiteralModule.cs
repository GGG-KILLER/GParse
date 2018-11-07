using System;
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
        where TokenTypeT : Enum
    {
        public delegate ExpressionNodeT NodeFactory ( Token<TokenTypeT> token );

        private readonly NodeFactory Factory;

        public LiteralModule ( NodeFactory factory )
        {
            this.Factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        public ExpressionNodeT ParsePrefix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, Token<TokenTypeT> readToken ) =>
            this.Factory ( readToken );
    }
}
