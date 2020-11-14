using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a postfix expression node.
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    /// <param name="operand">The operand expression.</param>
    /// <param name="operator">The operator token.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <param name="expression">The parsed expression if successful.</param>
    /// <returns>Whether the expression was able to be parsed.</returns>
    public delegate Boolean PostfixNodeFactory<TokenTypeT, ExpressionNodeT> (
        ExpressionNodeT operand,
        Token<TokenTypeT> @operator,
        DiagnosticList diagnostics,
        [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module that can parse a postfix operation with an
    /// operator that is composed of a single token.
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public class SingleTokenPostfixOperatorParselet<TokenTypeT, ExpressionNodeT> : IInfixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        private readonly PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <inheritdoc />
        public Int32 Precedence { get; }

        /// <summary>
        /// Initializes a new single token postfix operator parselet.
        /// </summary>
        /// <param name="precedence">The precedence of the operator this parselet is parsing.</param>
        /// <param name="factory">The method responsible for creating the postfix expression node.</param>
        public SingleTokenPostfixOperatorParselet ( Int32 precedence, PostfixNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence of the operator must be greater than 0" );

            this.Precedence = precedence;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse (
            IPrattParser<TokenTypeT, ExpressionNodeT> parser,
            ExpressionNodeT expression,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out ExpressionNodeT parsedExpression )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );
            if ( expression is null )
                throw new ArgumentNullException ( nameof ( expression ) );
            if ( diagnostics is null )
                throw new ArgumentNullException ( nameof ( diagnostics ) );

            return this.factory ( expression, parser.TokenReader.Consume ( ), diagnostics, out parsedExpression );
        }
    }
}
