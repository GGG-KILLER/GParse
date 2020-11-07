using System;
using System.Diagnostics.CodeAnalysis;
using GParse.IO;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// Defines the interface of a lexer module.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The module prefix
        /// </summary>
        String? Prefix { get; }

        /// <summary>
        /// Attempts to consume the contents in the reader as a token.
        /// </summary>
        /// <param name="reader">The reader to use when parsing the token.</param>
        /// <param name="diagnostics">The list of diagnostics</param>
        /// <param name="token">The parsed token (if any).</param>
        /// <returns></returns>
        Boolean TryConsume (
            ICodeReader reader,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out Token<TokenTypeT>? token );
    }
}
