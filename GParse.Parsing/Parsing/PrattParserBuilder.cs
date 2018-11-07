using System;
using System.Collections.Generic;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;
using GParse.Parsing.Parsing.Modules;

namespace GParse.Parsing.Parsing
{
    public class PrattParserBuilder<TokenTypeT, ExpressionNodeT> : IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        #region Modules

        private readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> PrefixModules = new Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> ( );

        private readonly Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> InfixModules = new Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> ( );

        #endregion Modules

        #region Register

        public void Register ( TokenTypeT tokenType, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, null)] = prefixModule;

        public void Register ( TokenTypeT tokenType, String ID, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, ID)] = prefixModule;

        public void Register ( TokenTypeT tokenType, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, null)] = infixModule;

        public void Register ( TokenTypeT tokenType, String ID, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, ID)] = infixModule;

        #endregion Register

        #region RegisterLiteral

        public void RegisterLiteral ( TokenTypeT tokenType, LiteralModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new LiteralModule<TokenTypeT, ExpressionNodeT> ( factory ) );

        public void RegisterLiteral ( TokenTypeT tokenType, String ID, LiteralModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new LiteralModule<TokenTypeT, ExpressionNodeT> ( factory ) );

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        public void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        public void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        public void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        public void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        public void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        public void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPostfixOperator

        public IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader ) =>
            new PrattParser<TokenTypeT, ExpressionNodeT> ( reader, this.PrefixModules, this.InfixModules );
    }
}
