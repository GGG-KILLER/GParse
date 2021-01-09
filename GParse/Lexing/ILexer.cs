namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a lexer
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public interface ILexer<TTokenType> : IReadOnlyLexer<TTokenType>, IRestorablePositionContainer
        where TTokenType : notnull
    {
        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TTokenType> Consume ( );
    }
}
