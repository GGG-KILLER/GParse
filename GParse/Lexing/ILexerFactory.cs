using System;
using GParse.IO;

namespace GParse.Lexing
{
    /// <summary>
    /// A lexer factory that creates lexers from string inputs and <see cref="ICodeReader"/>s.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerFactory<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Obtains a lexer for the provided <paramref name="input"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="input">The input to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns>The built lexer.</returns>
        ILexer<TokenTypeT> GetLexer ( String input, DiagnosticList diagnostics );

        /// <summary>
        /// Obtains a lexer for the provided <paramref name="reader"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="reader">The reader to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns></returns>
        ILexer<TokenTypeT> GetLexer ( ICodeReader reader, DiagnosticList diagnostics );
    }
}
