using System;
using GParse.Lexing;

namespace GParse.Parsing
{
    /// <summary>
    /// The base class for parsers.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public abstract class BaseParser<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// This is the <see cref="DiagnosticList"/> for this parser.
        /// </summary>
        protected DiagnosticList Diagnostics { get; }

        /// <inheritdoc />
        protected ITokenReader<TokenTypeT> TokenReader { get; }

        /// <summary>
        /// Initializes a new base parser.
        /// </summary>
        /// <param name="diagnostics"></param>
        /// <param name="tokenReader"></param>
        protected BaseParser ( DiagnosticList diagnostics, ITokenReader<TokenTypeT> tokenReader )
        {
            this.Diagnostics = diagnostics ?? throw new ArgumentNullException ( nameof ( diagnostics ) );
            this.TokenReader = tokenReader ?? throw new ArgumentNullException ( nameof ( tokenReader ) );
        }
    }
}