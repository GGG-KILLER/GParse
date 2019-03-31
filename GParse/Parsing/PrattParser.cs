using System;
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
        /// <summary>
        /// The registered <see cref="IPrefixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree;

        /// <summary>
        /// The registered <see cref="IInfixParselet{TokenTypeT, ExpressionNodeT}" />
        /// </summary>
        protected readonly PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree;

        /// <summary>
        /// The <see cref="Diagnostic" /> emitter provided to the constructor
        /// </summary>
        protected readonly IProgress<Diagnostic> diagnosticReporter;

        /// <summary>
        /// The parser's token reader
        /// </summary>
        public ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Initializes a pratt parser
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModuleTree"></param>
        /// <param name="infixModuleTree"></param>
        /// <param name="diagnosticEmitter"></param>
        protected internal PrattParser ( ITokenReader<TokenTypeT> tokenReader, PrattParserModuleTree<TokenTypeT, IPrefixParselet<TokenTypeT, ExpressionNodeT>> prefixModuleTree, PrattParserModuleTree<TokenTypeT, IInfixParselet<TokenTypeT, ExpressionNodeT>> infixModuleTree, IProgress<Diagnostic> diagnosticEmitter )
        {
            this.TokenReader        = tokenReader;
            this.prefixModuleTree   = prefixModuleTree;
            this.infixModuleTree    = infixModuleTree;
            this.diagnosticReporter = diagnosticEmitter;
        }

        #region PrattParser<TokenTypeT, ExpressionNodeT>

        #region ParseExpression

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Boolean TryParseExpression ( Int32 precedence, out ExpressionNodeT expression )
        {
            expression = default;
            var foundExpression = false;
            foreach ( IPrefixParselet<TokenTypeT, ExpressionNodeT> module in this.prefixModuleTree.GetSortedCandidates ( this.TokenReader ) )
            {
                SourceLocation start = this.TokenReader.Location;
                if ( module.TryParse ( this, this.diagnosticReporter, out expression ) )
                {
                    foundExpression = true;
                    break;
                }
                if ( this.TokenReader.Location != start )
                    this.TokenReader.Rewind ( start );
            }
            if ( !foundExpression )
                return false;

            Boolean couldParse;
            do
            {
                couldParse = false;
                foreach ( IInfixParselet<TokenTypeT, ExpressionNodeT> module in this.infixModuleTree.GetSortedCandidates ( this.TokenReader ) )
                {
                    SourceLocation start = this.TokenReader.Location;
                    if ( precedence < module.Precedence
                        && module.TryParse ( this, expression, this.diagnosticReporter, out ExpressionNodeT tmpExpr ) )
                    {
                        couldParse = true;
                        expression = tmpExpr;
                        break;
                    }
                    if ( this.TokenReader.Location != start )
                        this.TokenReader.Rewind ( start );
                }
            }
            while ( couldParse );

            return true;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Boolean TryParseExpression ( out ExpressionNodeT expression ) =>
            this.TryParseExpression ( 0, out expression );

        #endregion ParseExpression

        #endregion PrattParser<TokenTypeT, ExpressionNodeT>
    }
}
