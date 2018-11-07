using System;
using System.Collections.Generic;
using System.Text;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;

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

        public IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader ) =>
            new PrattParser<TokenTypeT, ExpressionNodeT> ( reader, this.PrefixModules, this.InfixModules );
    }
}
