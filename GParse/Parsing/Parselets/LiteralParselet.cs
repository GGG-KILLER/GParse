using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a literal node expression
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    /// <param name="token">The obtained token.</param>
    /// <param name="diagnostics">The diagnostic list to use when reporting new diagnostics.</param>
    /// <param name="expression">The resulting expression node.</param>
    /// <returns>Whether the expression was able to be parsed.</returns>
    public delegate Boolean LiteralNodeFactory<TokenTypeT, ExpressionNodeT> (
        Token<TokenTypeT> token,
        DiagnosticList diagnostics,
        [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TokenTypeT : notnull;

    /// <summary>
    /// A module for single token literals
    /// </summary>
    /// <typeparam name="TokenTypeT">The <see cref="Token{TokenTypeT}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public class LiteralParselet<TokenTypeT, ExpressionNodeT> : IPrefixParselet<TokenTypeT, ExpressionNodeT>
        where TokenTypeT : notnull
    {
        private readonly LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory;

        /// <summary>
        /// Initializes a new literal parselet.
        /// </summary>
        /// <param name="factory">The function that transforms a token into an expression node.</param>
        public LiteralParselet ( LiteralNodeFactory<TokenTypeT, ExpressionNodeT> factory )
        {
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

            return this.factory ( parser.TokenReader.Consume ( ), diagnostics, out parsedExpression );
        }
    }
}
