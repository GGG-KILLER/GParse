using System;
using System.Collections;
using System.Collections.Generic;

namespace GParse.Lexing
{
    /// <summary>
    /// An enumerator for the <see cref="ITokenReader{TokenTypeT}" /> that uses the
    /// <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" /> method to enumerate the tokens
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public struct TokenReaderEnumerator<TokenTypeT> : IEnumerator<Token<TokenTypeT>>
    {
        private ITokenReader<TokenTypeT> _tokenReader;
        private Int32 _offset;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        public TokenReaderEnumerator ( ITokenReader<TokenTypeT> reader )
        {
            this._tokenReader = reader;
            this._offset = -1;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Token<TokenTypeT> Current
        {
            get
            {
                if ( this._offset == -1 )
                    throw new InvalidOperationException ( "Enumeration has not started. Call MoveNext to start it" );
                if ( this._tokenReader == null )
                    throw new InvalidOperationException ( "Enumerator has already been disposed" );

                return this._tokenReader.Lookahead ( this._offset );
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                if ( this._offset == -1 )
                    throw new InvalidOperationException ( "Enumeration has not started. Call MoveNext to start it" );
                if ( this._tokenReader == null )
                    throw new InvalidOperationException ( "Enumerator has already been disposed" );

                return this._tokenReader.Lookahead ( this._offset );
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose ( )
        {
            this._tokenReader = null;
            this._offset = -1;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public Boolean MoveNext ( )
        {
            if ( this._tokenReader == null )
                return false;

            this._offset++;
            return true;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Reset ( ) => this._offset = -1;
    }
}
