using System;
using System.Diagnostics.CodeAnalysis;
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
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The this holds the tree of <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" /> to be used while parsing expressions.
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> _prefixModuleTree;

        /// <summary>
        /// This holds the tree of <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}"/> to be used while parsing expressions.
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> _infixModuleTree;

        /// <summary>
        /// This is the <see cref="IProgress{T}"/> reporter to which the parser should send <see cref="Diagnostic">Diagnostics</see> to.
        /// </summary>
        protected readonly DiagnosticList _diagnostics;

        /// <inheritdoc />
        public ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Initializes a pratt parser
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModuleTree"></param>
        /// <param name="infixModuleTree"></param>
        /// <param name="diagnostics"></param>
        protected internal PrattParser (
            ITokenReader<TokenTypeT> tokenReader,
            PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree,
            PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree,
            DiagnosticList diagnostics )
        {
            this.TokenReader = tokenReader ?? throw new ArgumentNullException ( nameof ( tokenReader ) );
            this._prefixModuleTree = prefixModuleTree ?? throw new ArgumentNullException ( nameof ( prefixModuleTree ) );
            this._infixModuleTree = infixModuleTree ?? throw new ArgumentNullException ( nameof ( infixModuleTree ) );
            this._diagnostics = diagnostics ?? throw new ArgumentNullException ( nameof ( diagnostics ) );
        }

        #region PrattParser<TokenTypeT, ExpressionNodeT>

        #region ParseExpression

        /// <inheritdoc />
        public virtual Boolean TryParseExpression ( Int32 minPrecedence, [NotNullWhen ( true )] out ExpressionNodeT expression )
        {
            expression = default!;
            var foundExpression = false;
            foreach ( IPrefixParselet<TokenTypeT, ExpressionNodeT> module in this._prefixModuleTree.GetSortedCandidates ( this.TokenReader ) )
            {
                var start = this.TokenReader.Position;
                if ( module.TryParse ( this, this._diagnostics, out expression ) )
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
                foreach ( IInfixParselet<TokenTypeT, ExpressionNodeT> module in this._infixModuleTree.GetSortedCandidates ( this.TokenReader ) )
                {
                    var start = this.TokenReader.Position;
                    if ( minPrecedence < module.Precedence
                        && module.TryParse ( this, expression, this._diagnostics, out ExpressionNodeT tmpExpr ) )
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

        #endregion PrattParser<TokenTypeT, ExpressionNodeT>
    }
}
