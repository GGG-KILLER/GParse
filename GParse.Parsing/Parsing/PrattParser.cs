using System;
using System.Collections.Generic;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing;
using GParse.Parsing.Abstractions.Parsing.Modules;
using GParse.Parsing.Parsing.Errors;

namespace GParse.Parsing.Parsing
{
    /// <summary>
    /// Implements the modular pratt expression parser
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParser<TokenTypeT, ExpressionNodeT> : IPrattParser<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : Enum
    {
        #region Modules

        /// <summary>
        /// The registered <see cref="IPrefixModule{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> PrefixModules;

        /// <summary>
        /// The registered <see cref="IInfixModule{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> InfixModules;

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
        protected internal PrattParser ( ITokenReader<TokenTypeT> tokenReader, Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> prefixModules, Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> infixModules )
        {
            this.TokenReader = tokenReader;
            this.PrefixModules = prefixModules;
            this.InfixModules = infixModules;
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
            return this.InfixModules.TryGetValue ( (peek.Type, peek.ID), out IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) || this.InfixModules.TryGetValue ( (peek.Type, null), out infixModule )
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

            if ( !this.PrefixModules.TryGetValue ( (readToken.Type, readToken.ID), out IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule ) && !this.PrefixModules.TryGetValue ( (readToken.Type, null), out prefixModule ) )
                throw new UnableToParseTokenException<TokenTypeT> ( readToken.Range.Start, readToken, $"Cannot parse '{readToken.Raw}'" );

            ExpressionNodeT leftHandSide = prefixModule.ParsePrefix ( this, readToken );

            while ( precedence < this.GetPrecedence ( ) )
            {
                readToken = this.TokenReader.Consume ( );
                if ( !this.InfixModules.TryGetValue ( (readToken.Type, readToken.ID), out IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) )
                    infixModule = this.InfixModules[(readToken.Type, null)];

                leftHandSide = infixModule.ParseInfix ( this, leftHandSide, readToken );
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
