using System;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a read-only lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface IReadOnlyLexer<TokenTypeT> : IPositionContainer
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Whether the lexer is at the end of the file
        /// </summary>
        Boolean EndOfFile { get; }

        /// <summary>
        /// Returns the next token without advancing in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Peek ( );
    }
}
