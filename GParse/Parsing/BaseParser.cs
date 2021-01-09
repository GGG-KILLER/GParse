using System;
using GParse.Lexing;

namespace GParse.Parsing
{
    /// <summary>
    /// The base class for parsers.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public abstract class BaseParser<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// This is the <see cref="DiagnosticList"/> for this parser.
        /// </summary>
        protected DiagnosticList Diagnostics { get; }

        /// <inheritdoc />
        protected ITokenReader<TTokenType> TokenReader { get; }

        /// <summary>
        /// Initializes a new base parser.
        /// </summary>
        /// <param name="diagnostics"></param>
        /// <param name="tokenReader"></param>
        protected BaseParser ( DiagnosticList diagnostics, ITokenReader<TTokenType> tokenReader )
        {
            this.Diagnostics = diagnostics ?? throw new ArgumentNullException ( nameof ( diagnostics ) );
            this.TokenReader = tokenReader ?? throw new ArgumentNullException ( nameof ( tokenReader ) );
        }
    }
}