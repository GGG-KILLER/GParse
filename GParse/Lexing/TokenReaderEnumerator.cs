using System;
using System.Collections;
using System.Collections.Generic;

namespace GParse.Lexing
{
    /// <summary>
    /// An enumerator for the <see cref="ITokenReader{TTokenType}" /> that uses the
    /// <see cref="ITokenReader{TTokenType}.Lookahead(Int32)" /> method to enumerate the tokens
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public struct TokenReaderEnumerator<TTokenType> : IEnumerator<Token<TTokenType>>
        where TTokenType : notnull
    {
        private ITokenReader<TTokenType> _tokenReader;
        private Int32 _offset;

        /// <inheritdoc />
        public TokenReaderEnumerator(ITokenReader<TTokenType> reader)
        {
            this._tokenReader = reader;
            this._offset = -1;
        }

        /// <inheritdoc />
        public Token<TTokenType> Current
        {
            get
            {
                if (this._offset == -1)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext to start it");
                if (this._tokenReader == null)
                    throw new InvalidOperationException("Enumerator has already been disposed");

                return this._tokenReader.Lookahead(this._offset);
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                if (this._offset == -1)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext to start it");
                if (this._tokenReader == null)
                    throw new InvalidOperationException("Enumerator has already been disposed");

                return this._tokenReader.Lookahead(this._offset);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._tokenReader = null!;
            this._offset = -1;
        }

        /// <inheritdoc />
        public Boolean MoveNext()
        {
            if (this._tokenReader == null)
                return false;

            this._offset++;
            return true;
        }

        /// <inheritdoc />
        public void Reset() => this._offset = -1;
    }
}