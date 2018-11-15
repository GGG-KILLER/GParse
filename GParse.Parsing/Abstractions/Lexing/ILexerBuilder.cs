using System;
using GParse.Common.IO;

namespace GParse.Parsing.Abstractions.Lexing
{
    /// <summary>
    /// Defines the interface of a
    /// <see cref="ILexer{TokenTypeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerBuilder<TokenTypeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// Adds a module to this builder
        /// </summary>
        /// <param name="module"></param>
        void AddModule ( ILexerModule<TokenTypeT> module );

        /// <summary>
        /// Builds a lexer with <paramref name="input" /> as the stream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ILexer<TokenTypeT> BuildLexer ( String input );

        /// <summary>
        /// Builds a lexer with <paramref name="reader" /> as the stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        ILexer<TokenTypeT> BuildLexer ( SourceCodeReader reader );
    }
}
