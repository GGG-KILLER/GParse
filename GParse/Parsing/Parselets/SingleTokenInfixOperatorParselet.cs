using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Lexing;

namespace GParse.Parsing.Parselets
{
    /// <summary>
    /// A delegate that will attempt to create a infix expression node
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    /// <param name="left">The expression on the left side of the operator.</param>
    /// <param name="op">The operator token.</param>
    /// <param name="right">The expression on the right side of the operator.</param>
    /// <param name="expression">The resulting parsed expression.</param>
    /// <returns>Whether it was possible to parse the expression or not.</returns>
    public delegate Boolean InfixNodeFactory<TTokenType, ExpressionNodeT> (
        ExpressionNodeT left,
        Token<TTokenType> op,
        ExpressionNodeT right,
        [NotNullWhen ( true )] out ExpressionNodeT expression )
        where TTokenType : notnull;

    /// <summary>
    /// A module that can parse an infix operation with an operator composed of a single token
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="ExpressionNodeT">The base type of expression nodes.</typeparam>
    public class SingleTokenInfixOperatorParselet<TTokenType, ExpressionNodeT> : IInfixParselet<TTokenType, ExpressionNodeT>
        where TTokenType : notnull
    {
        private readonly Boolean isRightAssociative;
        private readonly InfixNodeFactory<TTokenType, ExpressionNodeT> factory;

        /// <inheritdoc />
        public Int32 Precedence { get; }

        /// <summary>
        /// Initializes a new single token infix operator parselet.
        /// </summary>
        /// <param name="precedence">The operator's precedence.</param>
        /// <param name="isRightAssociative">Whether the operator is right-associative.</param>
        /// <param name="factory">The method that transforms the infix operation into an expression node.</param>
        public SingleTokenInfixOperatorParselet (
            Int32 precedence,
            Boolean isRightAssociative,
            InfixNodeFactory<TTokenType, ExpressionNodeT> factory )
        {
            this.Precedence = precedence;
            this.isRightAssociative = isRightAssociative;
            this.factory = factory ?? throw new ArgumentNullException ( nameof ( factory ) );
        }

        /// <inheritdoc />
        public Boolean TryParse (
            IPrattParser<TTokenType, ExpressionNodeT> parser,
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

            parsedExpression = default!;
            Token<TTokenType> op = parser.TokenReader.Consume ( );

            // We decrease the precedence by one on right-associative operators because the minimum
            // precedence passed to TryParseExpression is exclusive (meaning that the precedence of the
            // infix parselets must be higher than the one we pass it.
            // TODO: Check if this cannot create bugs with other operators that have the same precedence.
            Int32 minPrecedence;
            if ( this.isRightAssociative )
                minPrecedence = this.Precedence - 1;
            else
                minPrecedence = this.Precedence;

            if ( parser.TryParseExpression ( minPrecedence, out ExpressionNodeT nextExpr ) )
                return this.factory ( expression, op, nextExpr, out parsedExpression );
            else
                return false;
        }
    }
}
