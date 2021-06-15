using System;
using GParse.Lexing;
using GParse.Parsing.Parselets;
using Tsu;

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
        protected internal PrattParser(
            ITokenReader<TTokenType> tokenReader,
            PrattParserModuleTree<TTokenType, IPrefixParselet<TTokenType, TExpressionNode>> prefixModuleTree,
            PrattParserModuleTree<TTokenType, IInfixParselet<TTokenType, TExpressionNode>> infixModuleTree,
            DiagnosticList diagnostics)
            : base(diagnostics, tokenReader)
        {
            PrefixModuleTree = prefixModuleTree ?? throw new ArgumentNullException(nameof(prefixModuleTree));
            InfixModuleTree = infixModuleTree ?? throw new ArgumentNullException(nameof(infixModuleTree));
        }

        #region PrattParser<TTokenType, TExpressionNode>

        #region ParseExpression

        /// <inheritdoc />
        public virtual Option<TExpressionNode> ParseExpression(int minPrecedence)
        {
            var foundExpression = false;
            var expressionOpt = Option.None<TExpressionNode>();
            foreach (var module in PrefixModuleTree.GetSortedCandidates(TokenReader))
            {
                var start = TokenReader.Position;
                expressionOpt = module.Parse(this, Diagnostics);
                if (expressionOpt.IsSome)
                {
                    foundExpression = true;
                    break;
                }
                TokenReader.Restore(start);
            }
            if (!foundExpression)
                return Option.None<TExpressionNode>();

            bool couldParse;
            do
            {
                couldParse = false;
                foreach (var module in InfixModuleTree.GetSortedCandidates(TokenReader))
                {
                    var start = TokenReader.Position;
                    if (minPrecedence < module.Precedence)
                    {
                        var result = module.Parse(this, expressionOpt.Value, Diagnostics);
                        if (result.IsSome)
                        {
                            couldParse = true;
                            expressionOpt = result;
                            break;
                        }
                    }
                    TokenReader.Restore(start);
                }
            }
            while (couldParse);

            return expressionOpt;
        }

        /// <inheritdoc />
        public virtual Option<TExpressionNode> ParseExpression() => ParseExpression(0);

        #endregion ParseExpression

        #endregion PrattParser<TTokenType, TExpressionNode>
    }
}