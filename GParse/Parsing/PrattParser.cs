using System;
using System.Collections.Generic;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Implements the modular pratt expression parser
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParser<TokenTypeT, ExpressionNodeT> : IPrattParser<TokenTypeT, ExpressionNodeT>
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixParselet<TokenTypeT, ExpressionNodeT>> PrefixModules;

        /// <summary>
        /// The registered <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IInfixParselet<TokenTypeT, ExpressionNodeT>> InfixModules;

        /// <summary>
        /// The <see cref="Diagnostic" /> emitter provided to the constructor
        /// </summary>
        protected readonly IProgress<Diagnostic> DiagnosticEmitter;

        #endregion Modules

        /// <summary>
        /// The parser's token reader
        /// </summary>
        public ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Initializes a pratt parser
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModules"></param>
        /// <param name="infixModules"></param>
        /// <param name="diagnosticEmitter"></param>
        protected internal PrattParser ( ITokenReader<TokenTypeT> tokenReader, Dictionary<(TokenTypeT tokenType, String id), IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModules, Dictionary<(TokenTypeT tokenType, String id), IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModules, IProgress<Diagnostic> diagnosticEmitter )
        {
            this.TokenReader = tokenReader;
            this.PrefixModules = prefixModules;
            this.InfixModules = infixModules;
            this.DiagnosticEmitter = diagnosticEmitter;
        }

        #region PrattParser<TokenTypeT, ExpressionNodeT>

        #region ParseExpression

        /// <summary>
        /// Returns the precedence of the element ahead
        /// </summary>
        /// <returns></returns>
        protected virtual Int32 GetPrecedence ( )
        {
            Token<TokenTypeT> peek = this.TokenReader.Lookahead ( 0 );
            return this.InfixModules.TryGetValue ( (peek.Type, peek.ID), out IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule ) || this.InfixModules.TryGetValue ( (peek.Type, null), out infixModule )
                ? infixModule.Precedence
                : 0;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="precedence"></param>
        /// <returns></returns>
        public virtual ExpressionNodeT ParseExpression ( Int32 precedence )
        {
            Token<TokenTypeT> readToken = this.TokenReader.Consume ( );

            if ( !this.PrefixModules.TryGetValue ( (readToken.Type, readToken.ID), out IPrefixParselet<TokenTypeT, ExpressionNodeT> prefixModule ) && !this.PrefixModules.TryGetValue ( (readToken.Type, null), out prefixModule ) )
                throw new UnableToParseTokenException<TokenTypeT> ( readToken.Range, readToken, $"Cannot parse '{readToken.Raw}'" );

            ExpressionNodeT leftHandSide = prefixModule.ParsePrefix ( this, readToken, this.DiagnosticEmitter );

            while ( precedence < this.GetPrecedence ( ) )
            {
                readToken = this.TokenReader.Lookahead ( );
                if ( !this.InfixModules.TryGetValue ( (readToken.Type, readToken.ID), out IInfixParselet<TokenTypeT, ExpressionNodeT> infixModule ) && !this.InfixModules.TryGetValue ( (readToken.Type, null), out infixModule ) )
                    break;
                this.TokenReader.Consume ( );

                leftHandSide = infixModule.ParseInfix ( this, leftHandSide, readToken, this.DiagnosticEmitter );
            }

            return leftHandSide;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public virtual ExpressionNodeT ParseExpression ( ) => this.ParseExpression ( 0 );

        #endregion ParseExpression

        #endregion PrattParser<TokenTypeT, ExpressionNodeT>
    }
}
