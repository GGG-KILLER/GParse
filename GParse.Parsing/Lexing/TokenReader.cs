using System;
using System.Collections;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public class TokenReader<TokenTypeT> : ITokenReader<TokenTypeT>
        where TokenTypeT : Enum
    {
        private readonly List<Token<TokenTypeT>> TokenCache;
        private readonly Object TokenCacheLock = new Object ( );

        /// <summary>
        /// The parser's Lexer instance
        /// </summary>
        protected readonly ILexer<TokenTypeT> Lexer;

        public TokenReader ( ILexer<TokenTypeT> lexer ) : this ( lexer, 1 )
        {
        }

        public TokenReader ( ILexer<TokenTypeT> lexer, Int32 maxLookaheadOffset )
        {
            this.Lexer = lexer;
            this.TokenCache = new List<Token<TokenTypeT>> ( maxLookaheadOffset );
        }

        private void CacheTokens ( Int32 count )
        {
            while ( count-- > 0 )
                this.TokenCache.Add ( this.Lexer.Consume ( ) );
        }

        #region ITokenReader<TokenTypeT>

        /// <summary>
        /// The location of the parser
        /// </summary>
        public SourceLocation Location
        {
            get
            {
                lock ( this.TokenCacheLock )
                    return this.TokenCache.Count > 0 ? this.TokenCache[0].Range.Start : this.Lexer.Location;
            }
        }

        public Token<TokenTypeT> Lookahead ( Int32 offset = 0 )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count <= offset )
                    this.CacheTokens ( offset - this.TokenCache.Count + 1 );

                return this.TokenCache[offset];
            }
        }

        public Token<TokenTypeT> Consume ( )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count < 1 )
                    this.CacheTokens ( 1 );

                Token<TokenTypeT> tok = this.TokenCache[0];
                this.TokenCache.RemoveAt ( 0 );
                return tok;
            }
        }

        public void Skip ( Int32 count )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count <= count )
                {
                    count -= this.TokenCache.Count;
                    this.TokenCache.Clear ( );
                    while ( count-- > 0 )
                        this.Lexer.Consume ( );
                }
                else
                {
                    this.TokenCache.RemoveRange ( 0, count );
                }
            }
        }

        #region Accept

        public Boolean Accept ( String ID, out Token<TokenTypeT> token )
        {
            if ( this.Lookahead ( ).ID == ID )
            {
                token = this.Consume ( );
                return true;
            }
            token = null;
            return false;
        }

        public Boolean Accept ( String ID ) => this.Accept ( ID, out _ );

        public Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.Lookahead ( ).Type.Equals ( type ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = null;
            return false;
        }

        public Boolean Accept ( TokenTypeT type ) => this.Accept ( type, out _ );

        public Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.Lookahead ( ).Type.Equals ( type ) && this.Lookahead ( ).ID == ID )
            {
                token = this.Consume ( );
                return true;
            }
            token = null;
            return false;
        }

        public Boolean Accept ( String ID, TokenTypeT type ) => this.Accept ( ID, type, out _ );

        #endregion Accept

        #region Expect

        public Token<TokenTypeT> Expect ( String ID )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || peek.ID != ID )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID} but got {peek?.ID ?? "EOF"} instead." );
            return this.Consume ( );
        }

        public Token<TokenTypeT> Expect ( TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || !peek.Type.Equals ( type ) )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {type} but got {peek?.Type.ToString ( ) ?? "EOF"} instead." );
            return this.Consume ( );
        }

        public Token<TokenTypeT> Expect ( String ID, TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || !peek.Type.Equals ( type ) || peek.ID != ID )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID}+{type} but got a {peek?.ID ?? "EOF"}+{peek?.Type.ToString ( ) ?? "EOF"}" );
            return this.Consume ( );
        }

        #endregion Expect

        #endregion ITokenReader<TokenTypeT>

        public IEnumerator<Token<TokenTypeT>> GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );

        IEnumerator IEnumerable.GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );
    }
}
