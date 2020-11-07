using System;
using System.Collections.Immutable;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing
{
    /// <summary>
    /// The base class for lexers.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public abstract class BaseLexer<TokenTypeT> : ILexer<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The reader being used by the lexer
        /// </summary>
        protected readonly ICodeReader _reader;

        /// <summary>
        /// The <see cref="Diagnostic" /> emmiter.
        /// </summary>
        protected readonly DiagnosticList _diagnostics;

        /// <summary>
        /// Initializes a new lexer.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        protected BaseLexer ( ICodeReader reader, DiagnosticList diagnostics )
        {
            this._reader = reader;
            this._diagnostics = diagnostics;
        }

        /// <summary>
        /// Consumes the next token in the stream.
        /// </summary>
        /// <returns></returns>
        protected abstract Token<TokenTypeT> GetNextToken ( );

        /// <summary>
        /// Retrieves the first meaningful token while accumulating trivia
        /// </summary>
        /// <returns></returns>
        protected virtual Token<TokenTypeT> GetNextNonTriviaToken ( )
        {
            Token<TokenTypeT> tok;
            ImmutableArray<Token<TokenTypeT>>.Builder triviaBuilder = ImmutableArray.CreateBuilder<Token<TokenTypeT>> ( );
            while ( ( tok = this.GetNextToken ( ) ).IsTrivia )
                triviaBuilder.Add ( tok );

            return new Token<TokenTypeT> ( tok.Id, tok.Type, tok.Range, false, triviaBuilder.ToImmutable ( ), tok.Value, tok.Text );
        }

        #region IReadOnlyLexer

        /// <inheritdoc/>
        public Int32 Length => this._reader.Length;

        /// <inheritdoc/>
        public Int32 Position => this._reader.Position;

        /// <inheritdoc />
        public Boolean EndOfFile => this.Position == this.Length;

        /// <inheritdoc />
        public SourceLocation GetLocation ( ) => this._reader.GetLocation ( );

        /// <inheritdoc/>
        public SourceLocation GetLocation ( Int32 position ) => this._reader.GetLocation ( position );

        /// <inheritdoc/>
        public SourceLocation GetLocation ( Range<Int32> range ) => this._reader.GetLocation ( range );

        #endregion IReadOnlyLexer

        #region Lexer

        private Token<TokenTypeT>? _cachedToken;

        /// <inheritdoc />
        public Token<TokenTypeT> Peek ( )
        {
            if ( this._cachedToken is null )
            {
                var pos = this._reader.Position;
                this._cachedToken = this.Consume ( );
                this._reader.Restore ( pos );
            }

            return this._cachedToken;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> Consume ( )
        {
            if ( this._cachedToken is not null )
            {
                Token<TokenTypeT> tok = this._cachedToken;
                this._cachedToken = null;
                return tok;
            }

            return this.GetNextNonTriviaToken ( );
        }

        /// <inheritdoc/>
        public void Restore ( Int32 position ) => this._reader.Restore ( position );

        /// <inheritdoc/>
        public void Restore ( SourceLocation location ) => this._reader.Restore ( location );

        #endregion ILexer
    }
}