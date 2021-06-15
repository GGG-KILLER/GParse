using System;
using System.Collections.Immutable;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing
{
    /// <summary>
    /// The base class for lexers.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public abstract class BaseLexer<TTokenType> : ILexer<TTokenType>
        where TTokenType : notnull
    {
        private Token<TTokenType>? _cachedToken;

        #region IReadOnlyLexer

        /// <inheritdoc/>
        public Int32 Length => this.Reader.Length;

        /// <inheritdoc/>
        public Int32 Position => this.Reader.Position;

        /// <inheritdoc />
        public Boolean EndOfFile => this.Position == this.Length;

        #endregion IReadOnlyLexer

        /// <summary>
        /// The reader being used by the lexer
        /// </summary>
        protected ICodeReader Reader { get; }

        /// <summary>
        /// The <see cref="Diagnostic" /> emmiter.
        /// </summary>
        protected DiagnosticList Diagnostics { get; }

        /// <summary>
        /// Initializes a new lexer.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        protected BaseLexer(ICodeReader reader, DiagnosticList diagnostics)
        {
            this.Reader = reader;
            this.Diagnostics = diagnostics;
        }

        /// <summary>
        /// Consumes the next token in the stream.
        /// </summary>
        /// <returns></returns>
        protected abstract Token<TTokenType> GetNextToken();

        /// <summary>
        /// Retrieves the first meaningful token while accumulating trivia
        /// </summary>
        /// <returns></returns>
        protected virtual Token<TTokenType> GetNextNonTriviaToken()
        {
            Token<TTokenType> tok;
            ImmutableArray<Token<TTokenType>>.Builder triviaBuilder = ImmutableArray.CreateBuilder<Token<TTokenType>>();
            while ((tok = this.GetNextToken()).IsTrivia)
                triviaBuilder.Add(tok);

            return tok.WithTrivia(triviaBuilder.ToImmutable());
        }

        #region IReadOnlyLexer

        /// <inheritdoc />
        public Token<TTokenType> Peek()
        {
            if (this._cachedToken is null)
            {
                var pos = this.Reader.Position;
                this._cachedToken = this.Consume();
                this.Reader.Restore(pos);
            }

            return this._cachedToken;
        }

        /// <inheritdoc />
        public SourceLocation GetLocation() => this.Reader.GetLocation();

        /// <inheritdoc/>
        public SourceLocation GetLocation(Int32 position) => this.Reader.GetLocation(position);

        /// <inheritdoc/>
        public SourceRange GetLocation(Range<Int32> range) => this.Reader.GetLocation(range);

        #endregion IReadOnlyLexer

        #region ILexer

        /// <inheritdoc />
        public Token<TTokenType> Consume()
        {
            if (this._cachedToken is not null)
            {
                Token<TTokenType> tok = this._cachedToken;
                this._cachedToken = null;
                return tok;
            }

            return this.GetNextNonTriviaToken();
        }

        /// <inheritdoc/>
        public void Restore(Int32 position) => this.Reader.Restore(position);

        /// <inheritdoc/>
        public void Restore(SourceLocation location) => this.Reader.Restore(location);

        #endregion ILexer
    }
}