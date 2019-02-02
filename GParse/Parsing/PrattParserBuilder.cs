﻿using System;
using System.Collections.Generic;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Stores all modules that compose a <see cref="PrattParser{TokenTypeT, ExpressionNodeT}" />
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParserBuilder<TokenTypeT, ExpressionNodeT> : IPrattParserBuilder<TokenTypeT, ExpressionNodeT>
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixParselet<TokenTypeT, ExpressionNodeT>> PrefixModules = new Dictionary<(TokenTypeT tokenType, String id), IPrefixParselet<TokenTypeT, ExpressionNodeT>> ( );

        /// <summary>
        /// The registered <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IInfixParselet<TokenTypeT, ExpressionNodeT>> InfixModules = new Dictionary<(TokenTypeT tokenType, String id), IInfixParselet<TokenTypeT, ExpressionNodeT>> ( );

        #endregion Modules

        #region Register

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="prefixModule"></param>
        public virtual void Register ( TokenTypeT tokenType, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, null)] = prefixModule;

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="prefixModule"></param>
        public virtual void Register ( TokenTypeT tokenType, String ID, IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule ) =>
            this.PrefixModules[(tokenType, ID)] = prefixModule;

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="infixModule"></param>
        public void Register ( TokenTypeT tokenType, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, null)] = infixModule;

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="infixModule"></param>
        public void Register ( TokenTypeT tokenType, String ID, IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule ) =>
            this.InfixModules[(tokenType, ID)] = infixModule;

        #endregion Register

        #region RegisterLiteral

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, LiteralParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new LiteralParselet<TokenTypeT, ExpressionNodeT> ( factory ) );

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TokenTypeT tokenType, String ID, LiteralParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new LiteralParselet<TokenTypeT, ExpressionNodeT> ( factory ) );

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, Boolean isRightAssociative, SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenInfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, Int32 precedence, SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, new SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public void RegisterSingleTokenPostfixOperator ( TokenTypeT tokenType, String ID, Int32 precedence, SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT>.NodeFactory factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Creates a parser that will read from the
        /// <paramref name="reader" /> provided
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public virtual IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader, IProgress<Diagnostic> diagnosticEmitter ) =>
            new PrattParser<TokenTypeT, ExpressionNodeT> ( reader, this.PrefixModules, this.InfixModules, diagnosticEmitter );
    }
}