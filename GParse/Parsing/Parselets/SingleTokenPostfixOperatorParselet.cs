﻿using System;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A module that can parse a postfix operation with an
    /// operator that is composed of a single token
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> : IInfixParselet<TokenTypeT, ExpressionNodeT>
    {
        /// <summary>
        /// Defines the interface for a node factory
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public delegate ExpressionNodeT NodeFactory ( ExpressionNodeT value, Token<TokenTypeT> @operator );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Int32 Precedence { get; }

        private readonly NodeFactory Factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPostfixOperatorParselet ( Int32 precedence, NodeFactory factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence of the operator must be greater than 0" );

            this.Precedence = precedence;
            this.Factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="leftHandSide"></param>
        /// <param name="readToken"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public ExpressionNodeT ParseInfix ( IPrattParser<TokenTypeT, ExpressionNodeT> parser, ExpressionNodeT leftHandSide, Token<TokenTypeT> readToken, IProgress<Diagnostic> diagnosticEmitter ) =>
            this.Factory ( leftHandSide, readToken );
    }
}