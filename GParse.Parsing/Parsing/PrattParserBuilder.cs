using System;
using System.Collections.Generic;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;
using GParse.Parsing.Parsing.Modules;

namespace GParse.Parsing.Parsing
{
    /// <summary>
    /// Stores all modules that compose a <see cref="PrattParser{TokenTypeT, ExpressionNodeT}"/>
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParserBuilder<TokenTypeT, ExpressionNodeT> : IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixModule{TokenTypeT, ExpressionNodeT}"/>
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> PrefixModules = new Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> ( );

        /// <summary>
        /// The registered <see cref="IInfixModule{TokenTypeT, ExpressionNodeT}"/>
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> InfixModules = new Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> ( );

        #endregion Modules

        #region Register

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        public virtual void Register ( TokenTypeT tokenType, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, null)] = prefixModule;

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="prefixModule"></param>
        public virtual void Register ( TokenTypeT tokenType, String ID, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, ID)] = prefixModule;

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        public void Register ( TokenTypeT tokenType, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, null)] = infixModule;

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="infixModule"></param>
        public void Register ( TokenTypeT tokenType, String ID, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, ID)] = infixModule;

        #endregion Register

        #region RegisterLiteral

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, LiteralModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new LiteralModule<TokenTypeT, ExpressionNodeT> ( factory ) );

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, String ID, LiteralModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new LiteralModule<TokenTypeT, ExpressionNodeT> ( factory ) );

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPrefixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenInfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPostfixOperatorModule<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Creates a parser that will read from the
        /// <paramref name="reader" /> provided
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader ) =>
            new PrattParser<TokenTypeT, ExpressionNodeT> ( reader, this.PrefixModules, this.InfixModules );
    }
}
