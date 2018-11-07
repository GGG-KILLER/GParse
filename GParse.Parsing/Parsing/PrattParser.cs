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

        private readonly Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> PrefixModules;

        private readonly Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> InfixModules;

        #endregion Modules

        /// <summary>
        /// The parser's token reader
        /// </summary>
        public ITokenReader<TokenTypeT> TokenReader { get; }

        internal PrattParser ( ITokenReader<TokenTypeT> tokenReader, Dictionary<(TokenTypeT tokenType, String id), IPrefixModule<TokenTypeT, ExpressionNodeT>> prefixModules, Dictionary<(TokenTypeT tokenType, String id), IInfixModule<TokenTypeT, ExpressionNodeT>> infixModules )
        {
            this.TokenReader = tokenReader;
            this.PrefixModules = prefixModules;
            this.InfixModules = infixModules;
        }

        #region PrattParser<TokenTypeT, ExpressionNodeT>

        #region ParseExpression

        private Int32 GetPrecedence ( )
        {
            Token<TokenTypeT> peek = this.TokenReader.Lookahead ( 0 );
            return this.InfixModules.TryGetValue ( (peek.Type, peek.ID), out IInfixModule<TokenTypeT, ExpressionNodeT> infixModule ) || this.InfixModules.TryGetValue ( (peek.Type, null), out infixModule )
                ? infixModule.Precedence
                : 0;
        }

        public ExpressionNodeT ParseExpression ( Int32 precedence )
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

        public ExpressionNodeT ParseExpression ( ) => this.ParseExpression ( 0 );

        #endregion ParseExpression

        #endregion PrattParser<TokenTypeT, ExpressionNodeT>
    }
}
