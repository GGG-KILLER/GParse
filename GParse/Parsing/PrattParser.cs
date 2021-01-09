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
    /// <typeparam name="TExpressionNode"></typeparam>
    public class PrattParser<TTokenType, TExpressionNode> : BaseParser<TTokenType>, IPrattParser<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        /// <summary>
        /// The <see cref="IPrefixParselet{TTokenType, TExpressionNode}"/> module tree.
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, TExpressionNode>> PrefixModuleTree { get; }

        /// <summary>
        /// The <see cref="IInfixParselet{TTokenType, TExpressionNode}"/> module tree.
        /// </summary>
        protected PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, TExpressionNode>> InfixModuleTree { get; }

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
            PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, TExpressionNode>> prefixModuleTree,
            PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, TExpressionNode>> infixModuleTree,
            DiagnosticList diagnostics )
            : base ( diagnostics, tokenReader )
        {
            this.PrefixModuleTree = prefixModuleTree ?? throw new ArgumentNullException ( nameof ( prefixModuleTree ) );
            this.InfixModuleTree = infixModuleTree ?? throw new ArgumentNullException ( nameof ( infixModuleTree ) );
        }

        #region PrattParser<TTokenType, TExpressionNode>

        #region ParseExpression

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out TExpressionNode expression )
        {
            expression = default!;
            var foundExpression = false;
            foreach ( IPrefixParselet<TTokenType, TExpressionNode> module in this.PrefixModuleTree.GetSortedCandidates ( this.TokenReader ) )
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
                foreach ( IInfixParselet<TTokenType, TExpressionNode> module in this.InfixModuleTree.GetSortedCandidates ( this.TokenReader ) )
                {
                    var start = this.TokenReader.Position;
                    if ( minPrecedence < module.Precedence
                        && module.TryParse ( this, expression, this.Diagnostics, out TExpressionNode tmpExpr ) )
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
        public virtual Boolean TryParseExpression ( [NotNullWhen ( true )] out TExpressionNode expression ) =>
            this.TryParseExpression ( 0, out expression );

        #endregion ParseExpression

        #endregion PrattParser<TTokenType, TExpressionNode>
    }
}
