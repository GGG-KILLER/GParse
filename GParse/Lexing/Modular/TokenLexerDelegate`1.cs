using GParse.IO;
using Tsu;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// The delegate of a token parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="diagnostics"></param>
    /// <returns></returns>
    public delegate Option<Token<T>> TokenLexerDelegate<T>(ICodeReader reader, DiagnosticList diagnostics)
        where T : notnull;
}
