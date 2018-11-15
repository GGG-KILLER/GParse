using System;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    /// <summary>
    /// Defines the interface of a lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexer<TokenTypeT> : IReadOnlyLexer<TokenTypeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Consume ( );
    }
}
