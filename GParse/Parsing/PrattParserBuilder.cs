using System;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Stores all modules that compose a <see cref="PrattParser{TTokenType, TExpressionNode}" />
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="TExpressionNode"></typeparam>
    public class PrattParserBuilder<TTokenType, TExpressionNode> : IPrattParserBuilder<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixParselet{TTokenType, TExpressionNode}" />
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, TExpressionNode>> PrefixModuleTree { get; } = new();

        /// <summary>
        /// The registered <see cref="IInfixParselet{TTokenType, TExpressionNode}" />
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, TExpressionNode>> InfixModuleTree { get; } = new();

        #endregion Modules

        #region Register

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="parselet"></param>
        public virtual void Register(
            TTokenType tokenType,
            IPrefixParselet<TTokenType, TExpressionNode> parselet)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (parselet is null)
                throw new ArgumentNullException(nameof(parselet));

            PrefixModuleTree.AddModule(tokenType, parselet);
        }

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="parselet"></param>
        public virtual void Register(
            TTokenType tokenType,
            string id,
            IPrefixParselet<TTokenType, TExpressionNode> parselet)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            if (parselet is null)
                throw new ArgumentNullException(nameof(parselet));

            PrefixModuleTree.AddModule(tokenType, id, parselet);
        }

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="parselet"></param>
        public void Register(
            TTokenType tokenType,
            IInfixParselet<TTokenType, TExpressionNode> parselet)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (parselet is null)
                throw new ArgumentNullException(nameof(parselet));

            InfixModuleTree.AddModule(tokenType, parselet);
        }

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="parselet"></param>
        public void Register(
            TTokenType tokenType,
            string id,
            IInfixParselet<TTokenType, TExpressionNode> parselet)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            if (parselet is null)
                throw new ArgumentNullException(nameof(parselet));

            InfixModuleTree.AddModule(tokenType, id, parselet);
        }

        #endregion Register

        #region RegisterLiteral

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral(
            TTokenType tokenType,
            LiteralNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, new LiteralParselet<TTokenType, TExpressionNode>(factory));

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral(
            TTokenType tokenType,
            string ID,
            LiteralNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, ID, new LiteralParselet<TTokenType, TExpressionNode>(factory));

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator(
            TTokenType tokenType,
            int precedence,
            PrefixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, new SingleTokenPrefixOperatorParselet<TTokenType, TExpressionNode>(precedence, factory));

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator(
            TTokenType tokenType,
            string ID,
            int precedence,
            PrefixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, ID, new SingleTokenPrefixOperatorParselet<TTokenType, TExpressionNode>(precedence, factory));

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator(
            TTokenType tokenType,
            int precedence,
            bool isRightAssociative,
            InfixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, new SingleTokenInfixOperatorParselet<TTokenType, TExpressionNode>(precedence, isRightAssociative, factory));

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator(
            TTokenType tokenType,
            string ID,
            int precedence,
            bool isRightAssociative,
            InfixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, ID, new SingleTokenInfixOperatorParselet<TTokenType, TExpressionNode>(precedence, isRightAssociative, factory));

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPostfixOperator(
            TTokenType tokenType,
            int precedence,
            PostfixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, new SingleTokenPostfixOperatorParselet<TTokenType, TExpressionNode>(precedence, factory));

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public void RegisterSingleTokenPostfixOperator(
            TTokenType tokenType,
            string ID,
            int precedence,
            PostfixNodeFactory<TTokenType, TExpressionNode> factory) =>
            Register(tokenType, ID, new SingleTokenPostfixOperatorParselet<TTokenType, TExpressionNode>(precedence, factory));

        #endregion RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Creates a parser that will read from the <paramref name="reader" /> provided
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public virtual IPrattParser<TTokenType, TExpressionNode> CreateParser(
            ITokenReader<TTokenType> reader,
            DiagnosticList diagnostics) =>
            new PrattParser<TTokenType, TExpressionNode>(reader, PrefixModuleTree, InfixModuleTree, diagnostics);
    }
}