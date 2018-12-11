using System;
using System.Collections;
using System.Collections.Generic;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    /// <summary>
    /// An enumerator for the
    /// <see cref="ITokenReader{TokenTypeT}" /> that uses the
    /// <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" />
    /// method to enumerate the tokens
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public struct TokenReaderEnumerator<TokenTypeT> : IEnumerator<Token<TokenTypeT>>
    {
        private ITokenReader<TokenTypeT> TokenReader;
        private Int32 Offset;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        public TokenReaderEnumerator ( ITokenReader<TokenTypeT> reader )
        {
            this.TokenReader = reader;
            this.Offset = -1;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Token<TokenTypeT> Current
        {
            get
            {
                if ( this.Offset == -1 )
                    throw new InvalidOperationException ( "Enumeration has not started. Call MoveNext to start it" );
                if ( this.TokenReader == null )
                    throw new InvalidOperationException ( "Enumerator has already been disposed" );

                return this.TokenReader.Lookahead ( this.Offset );
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                if ( this.Offset == -1 )
                    throw new InvalidOperationException ( "Enumeration has not started. Call MoveNext to start it" );
                if ( this.TokenReader == null )
                    throw new InvalidOperationException ( "Enumerator has already been disposed" );

                return this.TokenReader.Lookahead ( this.Offset );
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose ( )
        {
            this.TokenReader = null;
            this.Offset = -1;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public Boolean MoveNext ( )
        {
            if ( this.TokenReader == null )
                return false;

            this.Offset++;
            return true;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Reset ( ) => this.Offset = -1;
    }
}
