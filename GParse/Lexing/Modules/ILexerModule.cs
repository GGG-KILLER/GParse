using System;
using GParse.IO;

namespace GParse.Lexing.Modules
{
    /// <summary>
    /// Defines the interface of a lexer module
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ILexerModule<TokenTypeT>
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The module prefix
        /// </summary>
        String Prefix { get; }

        /// <summary>
        /// Whether this module can consume what's left in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Boolean CanConsumeNext ( SourceCodeReader reader );

        /// <summary>
        /// Consume the next element in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader, IProgress<Diagnostic> diagnosticEmitter );
    }
}
