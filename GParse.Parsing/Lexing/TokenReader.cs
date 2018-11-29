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

        /// <summary>
        /// Saves N tokens from the lexer on the readahead cache
        /// </summary>
        /// <param name="count"></param>
        protected void CacheTokens ( Int32 count )
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

        #region IsAhead

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="tokenType">The wanted type</param>
        /// <param name="offset">The offset</param>
        /// <returns></returns>
        public Boolean IsAhead ( TokenTypeT tokenType, Int32 offset = 0 ) => this.Lookahead ( offset ).Type.Equals ( tokenType );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, Int32 offset = 0 )
        {
            TokenTypeT type = this.Lookahead(offset).Type;
            foreach ( TokenTypeT wtype in tokenTypes )
                if ( wtype.Equals ( type ) )
                    return true;
            return false;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsAhead ( String ID, Int32 offset = 0 ) => this.Lookahead ( offset ).ID.Equals ( ID );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 )
        {
            var aheadId = this.Lookahead ( offset ).ID;
            foreach ( var id in ids )
                if ( id.Equals ( aheadId ) )
                    return true;
            return false;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsAhead ( TokenTypeT tokenType, String id, Int32 offset = 0 ) =>
            this.IsAhead ( tokenType, offset ) && this.IsAhead ( id, offset );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 ) =>
            this.IsAhead ( tokenTypes, offset ) && this.IsAhead ( ids, offset );

        #endregion IsAhead

        #region Accept

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( String ID, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( ID ) )
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
        /// <param name="IDs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<String> IDs, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( IDs ) )
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
        /// <param name="IDs"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<String> IDs ) => this.Accept ( IDs, out _ );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( type ) )
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
        /// <param name="types"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<TokenTypeT> types, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( types ) )
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
        /// <param name="types"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<TokenTypeT> types ) => this.Accept ( types, out _ );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( type, ID ) )
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
        /// <param name="IDs"></param>
        /// <param name="types"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<String> IDs, IEnumerable<TokenTypeT> types, out Token<TokenTypeT> token )
        {
            if ( this.IsAhead ( types, IDs ) )
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public Boolean Accept ( IEnumerable<String> IDs, IEnumerable<TokenTypeT> types ) => this.Accept ( IDs, types, out _ );

        #endregion Accept

        #region Expect

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( String ID )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( ID ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected a {ID} but got {next?.ID ?? "unknown"} instead." );
            return next;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( IEnumerable<String> IDs )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( IDs ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected any ({String.Join ( ", ", IDs )}) but got {next?.ID ?? "unknown"}" );
            return next;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( TokenTypeT type )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( type ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected a {type} but got {next?.Type.ToString ( ) ?? "EOF"} instead." );
            return next;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( IEnumerable<TokenTypeT> types )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( types ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected any ({String.Join ( ", ", types )}) but got {next.Type}" );
            return next;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( String ID, TokenTypeT type )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( ID, type ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected a {ID}+{type} but got a {next?.ID ?? "EOF"}+{next?.Type.ToString ( ) ?? "EOF"}" );
            return next;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public Token<TokenTypeT> Expect ( IEnumerable<String> IDs, IEnumerable<TokenTypeT> types )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( IDs, types ) )
                throw new ParsingException ( next?.Range.Start ?? SourceLocation.Min, $"Expected any ({String.Join ( ", ", IDs )})+({String.Join ( ", ", types )}) but got {next?.ID ?? "unknown"}+{next?.Type.ToString ( ) ?? "unknown"}" );
            return next;
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
