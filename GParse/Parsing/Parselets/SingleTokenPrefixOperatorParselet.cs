using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a prefix expression node.
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TExpressionNode">The base type of expression nodes.</typeparam>
    /// <param name="operator">The operator token.</param>
    /// <param name="operand">The operand expression.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <param name="expression">The resulting parsed expression.</param>
    /// <returns></returns>
    public delegate Boolean PrefixNodeFactory<TTokenType, TExpressionNode> (
        Token<TTokenType> @operator,
        TExpressionNode operand,
        DiagnosticList diagnostics,
        [NotNullWhen ( true )] out TExpressionNode expression )
        where TTokenType : notnull;

    /// <summary>
    /// A module for single-token prefix operators
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    /// <typeparam name="TExpressionNode"></typeparam>
    public class SingleTokenPrefixOperatorParselet<TTokenType, TExpressionNode> : IPrefixParselet<TTokenType, TExpressionNode>
        where TTokenType : notnull
    {
        private readonly Int32 precedence;
        private readonly PrefixNodeFactory<TTokenType, TExpressionNode> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPrefixOperatorParselet ( Int32 precedence, PrefixNodeFactory<TTokenType, TExpressionNode> factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence must be a value greater than 0." );

            this.precedence = precedence;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse (
            IPrattParser<TTokenType, TExpressionNode> parser,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out TExpressionNode parsedExpression )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );
            if ( diagnostics is null )
                throw new ArgumentNullException ( nameof ( diagnostics ) );

            parsedExpression = default!;
            Token<TTokenType> prefix = parser.TokenReader.Consume ( );
            if ( parser.TryParseExpression ( this.precedence, out TExpressionNode expression ) )
                return this.factory ( prefix, expression, diagnostics, out parsedExpression );
            else
                return false;
        }
    }
}
