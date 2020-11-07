namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexer<TokenTypeT> : IReadOnlyLexer<TokenTypeT>, IRestorablePositionContainer
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Consume ( );
    }
}
