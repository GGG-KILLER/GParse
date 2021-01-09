using System;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Stores all modules that compose a <see cref="PrattParser{TTokenType, ExpressionNodeT}" />
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParserBuilder<TTokenType, ExpressionNodeT> : IPrattParserBuilder<TTokenType, ExpressionNodeT>
        where TTokenType : notnull
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixParselet{TTokenType, ExpressionNodeT}" />
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, ExpressionNodeT>> PrefixModuleTree { get; } = new ( );

        /// <summary>
        /// The registered <see cref="IInfixParselet{TTokenType, ExpressionNodeT}" />
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, ExpressionNodeT>> InfixModuleTree { get; } = new ( );

        #endregion Modules

        #region Register

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="parselet"></param>
        public virtual void Register ( TTokenType tokenType, IPrefixParselet<TTokenType, ExpressionNodeT> parselet )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( parselet is null )
                throw new ArgumentNullException ( nameof ( parselet ) );

            this.PrefixModuleTree.AddModule ( tokenType, parselet );
        }

        /// <summary>
        /// Registers a new prefix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="parselet"></param>
        public virtual void Register ( TTokenType tokenType, String id, IPrefixParselet<TTokenType, ExpressionNodeT> parselet )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( id is null )
                throw new ArgumentNullException ( nameof ( id ) );
            if ( parselet is null )
                throw new ArgumentNullException ( nameof ( parselet ) );

            this.PrefixModuleTree.AddModule ( tokenType, id, parselet );
        }

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="parselet"></param>
        public void Register ( TTokenType tokenType, IInfixParselet<TTokenType, ExpressionNodeT> parselet )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( parselet is null )
                throw new ArgumentNullException ( nameof ( parselet ) );

            this.InfixModuleTree.AddModule ( tokenType, parselet );
        }

        /// <summary>
        /// Registers a new infix module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="parselet"></param>
        public void Register ( TTokenType tokenType, String id, IInfixParselet<TTokenType, ExpressionNodeT> parselet )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( id is null )
                throw new ArgumentNullException ( nameof ( id ) );
            if ( parselet is null )
                throw new ArgumentNullException ( nameof ( parselet ) );

            this.InfixModuleTree.AddModule ( tokenType, id, parselet );
        }

        #endregion Register

        #region RegisterLiteral

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TTokenType tokenType, LiteralNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new LiteralParselet<TTokenType, ExpressionNodeT> ( factory ) );

        /// <summary>
        /// Registers a literal token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="factory"></param>
        public virtual void RegisterLiteral ( TTokenType tokenType, String ID, LiteralNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new LiteralParselet<TTokenType, ExpressionNodeT> ( factory ) );

        #endregion RegisterLiteral

        #region RegisterSingleTokenPrefixOperator

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TTokenType tokenType, Int32 precedence, PrefixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenPrefixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a prefix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPrefixOperator ( TTokenType tokenType, String ID, Int32 precedence, PrefixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPrefixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPrefixOperator

        #region RegisterSingleTokenInfixOperator

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TTokenType tokenType, Int32 precedence, Boolean isRightAssociative, InfixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenInfixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        /// <summary>
        /// Registers an infix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="isRightAssociative"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenInfixOperator ( TTokenType tokenType, String ID, Int32 precedence, Boolean isRightAssociative, InfixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenInfixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, isRightAssociative, factory ) );

        #endregion RegisterSingleTokenInfixOperator

        #region RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public virtual void RegisterSingleTokenPostfixOperator ( TTokenType tokenType, Int32 precedence, PostfixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, new SingleTokenPostfixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, factory ) );

        /// <summary>
        /// Registers a postfix operator composed of a single token
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public void RegisterSingleTokenPostfixOperator ( TTokenType tokenType, String ID, Int32 precedence, PostfixNodeFactory<TTokenType, ExpressionNodeT> factory ) =>
            this.Register ( tokenType, ID, new SingleTokenPostfixOperatorParselet<TTokenType, ExpressionNodeT> ( precedence, factory ) );

        #endregion RegisterSingleTokenPostfixOperator

        /// <summary>
        /// Creates a parser that will read from the <paramref name="reader" /> provided
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public virtual IPrattParser<TTokenType, ExpressionNodeT> CreateParser (
            ITokenReader<TTokenType> reader,
            DiagnosticList diagnostics ) =>
            new PrattParser<TTokenType, ExpressionNodeT> ( reader, this.PrefixModuleTree, this.InfixModuleTree, diagnostics );
    }
}
