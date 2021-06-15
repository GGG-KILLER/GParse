using GParse.IO;
using Tsu;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// Defines the interface of a lexer module.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public interface ILexerModule<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The module prefix
        /// </summary>
        string? Prefix { get; }

        /// <summary>
        /// Attempts to consume the contents in the reader as a token.
        /// </summary>
        /// <param name="reader">The reader to use when parsing the token.</param>
        /// <param name="diagnostics">The list of diagnostics</param>
        /// <returns>The parsed token (if successful).</returns>
        Option<Token<TTokenType>> TryConsume(ICodeReader reader, DiagnosticList diagnostics);
    }
}