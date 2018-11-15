using System;
using System.Collections;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    /// <summary>
    /// Implements the token reader interface
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class TokenReader<TokenTypeT> : ITokenReader<TokenTypeT>
        where TokenTypeT : Enum
    {
        private readonly List<Token<TokenTypeT>> TokenCache;
        private readonly Object TokenCacheLock = new Object ( );

        /// <summary>
        /// The parser's Lexer instance
        /// </summary>
        protected readonly ILexer<TokenTypeT> Lexer;

        /// <summary>
        /// Initializes a token with a cache of lookahead of 1 token
        /// </summary>
        /// <param name="lexer"></param>
        public TokenReader ( ILexer<TokenTypeT> lexer ) : this ( lexer, 1 )
        {
        }

        /// <summary>
        /// Initializes a token with a lookahead cache size of <paramref name="maxLookaheadOffset" />
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="maxLookaheadOffset"></param>
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
        /// <inheritdoc />
        /// </summary>
        public SourceLocation Location
        {
            get
            {
                lock ( this.TokenCacheLock )
                    return this.TokenCache.Count > 0 ? this.TokenCache[0].Range.Start : this.Lexer.Location;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Lookahead ( Int32 offset = 0 )
        {
            lock ( this.TokenCacheLock )
            {
                if ( this.TokenCache.Count <= offset )
                    this.CacheTokens ( offset - this.TokenCache.Count + 1 );

                return this.TokenCache[offset];
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="count"></param>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Boolean Accept ( String ID ) => this.Accept ( ID, out _ );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean Accept ( TokenTypeT type ) => this.Accept ( type, out _ );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean Accept ( String ID, TokenTypeT type ) => this.Accept ( ID, type, out _ );

        #endregion Accept

        #region Expect

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( String ID )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || peek.ID != ID )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID} but got {peek?.ID ?? "EOF"} instead." );
            return this.Consume ( );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || !peek.Type.Equals ( type ) )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {type} but got {peek?.Type.ToString ( ) ?? "EOF"} instead." );
            return this.Consume ( );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( String ID, TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.Lookahead ( );
            if ( peek == null || !peek.Type.Equals ( type ) || peek.ID != ID )
                throw new ParsingException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID}+{type} but got a {peek?.ID ?? "EOF"}+{peek?.Type.ToString ( ) ?? "EOF"}" );
            return this.Consume ( );
        }

        #endregion Expect

        #endregion ITokenReader<TokenTypeT>

        /// <summary>
        /// Returns an enumerator that uses
        /// <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" />
        /// to enumerate all tokens
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token<TokenTypeT>> GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );

        /// <summary>
        /// Returns an enumerator that uses
        /// <see cref="ITokenReader{TokenTypeT}.Lookahead(Int32)" />
        /// to enumerate all tokens
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator ( ) => new TokenReaderEnumerator<TokenTypeT> ( this );
    }
}
