using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a prefix expression node.
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    /// <param name="operator">The operator token.</param>
    /// <param name="operand">The operand expression.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <param name="expression">The resulting parsed expression.</param>
    /// <returns></returns>
    public delegate Boolean PrefixNodeFactory<TokenTypeT, ExpressionNodeT> (
        Token<TokenTypeT> @operator,
        ExpressionNodeT operand,
        DiagnosticList diagnostics,
        [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module for single-token prefix operators
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    /// <typeparam name="ExpressionNodeT"></typeparam>
    public class SingleTokenPrefixOperatorParselet<TokenTypeT, ExpressionNodeT> : IPrefixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        private readonly Int32 precedence;
        private readonly PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="factory"></param>
        public SingleTokenPrefixOperatorParselet ( Int32 precedence, PrefixNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "Precedence must be a value greater than 0." );

            this.precedence = precedence;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse (
            IPrattParser<TokenTypeT, ExpressionNodeT> parser,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out ExpressionNodeT parsedExpression )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );
            if ( diagnostics is null )
                throw new ArgumentNullException ( nameof ( diagnostics ) );

            parsedExpression = default!;
            Token<TokenTypeT> prefix = parser.TokenReader.Consume ( );
            if ( parser.TryParseExpression ( this.precedence, out ExpressionNodeT expression ) )
                return this.factory ( prefix, expression, diagnostics, out parsedExpression );
            else
                return false;
        }
    }
}
