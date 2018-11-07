using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public struct TokenReaderEnumerator<TokenTypeT> : IEnumerator<Token<TokenTypeT>> where TokenTypeT : Enum
    {
        private ITokenReader<TokenTypeT> TokenReader;
        private Int32 Offset;

        public TokenReaderEnumerator ( ITokenReader<TokenTypeT> reader )
        {
            this.TokenReader = reader;
            this.Offset = -1;
        }

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

        public void Dispose ( )
        {
            this.TokenReader = null;
            this.Offset = -1;
        }

        public Boolean MoveNext ( )
        {
            if ( this.TokenReader == null )
                return false;

            this.Offset++;
            return true;
        }

        public void Reset ( )
        {
            this.Offset = -1;
        }
    }
}
