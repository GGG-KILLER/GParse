using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;
using GParse.Parsing.Parselets;

namespace GParse.Parsing
{
    /// <summary>
    /// Implements the modular pratt expression parser
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class PrattParser<TTokenType, ExpressionNodeT> : BaseParser<TTokenType>, IPrattParser<TTokenType, ExpressionNodeT>
        where TTokenType : notnull
    {
        /// <summary>
        /// The <see cref="IPrefixParselet{TTokenType, ExpressionNodeT}"/> module tree.
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, ExpressionNodeT>> PrefixModuleTree { get; }

        /// <summary>
        /// The <see cref="IInfixParselet{TTokenType, ExpressionNodeT}"/> module tree.
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, ExpressionNodeT>> InfixModuleTree { get; }

        /// <inheritdoc/>
        public new ITokenReader<TTokenType> TokenReader => base.TokenReader;

        /// <summary>
        /// Initializes a new pratt parser
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModuleTree"></param>
        /// <param name="infixModuleTree"></param>
        /// <param name="diagnostics"></param>
        protected internal PrattParser (
            ITokenReader<TTokenType> tokenReader,
            PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, ExpressionNodeT>> prefixModuleTree,
            PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, ExpressionNodeT>> infixModuleTree,
            DiagnosticList diagnostics )
            : base ( diagnostics, tokenReader )
        {
            this.PrefixModuleTree = prefixModuleTree ?? throw new ArgumentNullException ( nameof ( prefixModuleTree ) );
            this.InfixModuleTree = infixModuleTree ?? throw new ArgumentNullException ( nameof ( infixModuleTree ) );
        }

        #region PrattParser<TTokenType, ExpressionNodeT>

        #region ParseExpression

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out ExpressionNodeT expression )
        {
            expression = default!;
            var foundExpression = false;
            foreach ( IPrefixParselet<TTokenType, ExpressionNodeT> module in this.PrefixModuleTree.GetSortedCandidates ( this.TokenReader ) )
            {
                var start = this.TokenReader.Position;
                if ( module.TryParse ( this, this.Diagnostics, out expression ) )
                {
                    foundExpression = true;
                    break;
                }
                if ( this.TokenReader.Position != start )
                    this.TokenReader.Restore ( start );
            }
            if ( !foundExpression )
                return false;

            Boolean couldParse;
            do
            {
                couldParse = false;
                foreach ( IInfixParselet<TTokenType, ExpressionNodeT> module in this.InfixModuleTree.GetSortedCandidates ( this.TokenReader ) )
                {
                    var start = this.TokenReader.Position;
                    if ( minPrecedence < module.Precedence
                        && module.TryParse ( this, expression, this.Diagnostics, out ExpressionNodeT tmpExpr ) )
                    {
                        couldParse = true;
                        expression = tmpExpr;
                        break;
                    }
                    if ( this.TokenReader.Position != start )
                        this.TokenReader.Restore ( start );
                }
            }
            while ( couldParse );

            return true;
        }

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( [NotNullWhen ( true )] out ExpressionNodeT expression ) =>
            this.TryParseExpression ( 0, out expression );

        #endregion ParseExpression

        #endregion PrattParser<TTokenType, ExpressionNodeT>
    }
}
