using System;
using GParse;
using GParse.IO;
using GParse.Lexing.Modules;

namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a
    /// <see cref="ILexer{TokenTypeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerBuilder<TokenTypeT>
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
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        ILexer<TokenTypeT> BuildLexer ( String input, IProgress<Diagnostic> diagnosticEmitter );

        /// <summary>
        /// Builds a lexer with <paramref name="reader" /> as the stream
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        ILexer<TokenTypeT> BuildLexer ( SourceCodeReader reader, IProgress<Diagnostic> diagnosticEmitter );
    }
}
