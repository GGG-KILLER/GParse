using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GParse.Errors;

namespace GParse.Lexing
{
    /// <summary>
    /// A token reader that reads tokens as requested and caches them
    /// when lookaheads are requested.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class TokenReader<TTokenType> : ITokenReader<TTokenType>
        where TTokenType : notnull
    {
        private readonly ImmutableArray<Token<TTokenType>> _tokens;

        /// <inheritdoc/>
        public Boolean EOF => this.Position == this.Length;

        /// <inheritdoc/>
        public Int32 Position { get; private set; }

        /// <inheritdoc/>
        public Int32 Length => this._tokens.Length;

        /// <summary>
        /// Reads the entire token stream
        /// </summary>
        /// <param name="lexer"></param>
        public TokenReader ( ILexer<TTokenType> lexer )
        {
            if ( lexer is null )
                throw new ArgumentNullException ( nameof ( lexer ) );

            this.Position = 0;
            ImmutableArray<Token<TTokenType>>.Builder tokens = ImmutableArray.CreateBuilder<Token<TTokenType>> ( );
            while ( !lexer.EndOfFile )
                tokens.Add ( lexer.Consume ( ) );
            this._tokens = tokens.ToImmutable ( );
        }

        #region ITokenReader<TTokenType>

        /// <inheritdoc />
        [SuppressMessage ( "Style", "IDE0056:Use index operator", Justification = "Not available on all target frameworks." )]
        [SuppressMessage ( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Valid for some target frameworks." )]
        public Token<TTokenType> Lookahead ( Int32 offset = 0 ) =>
            this.Position + offset < this.Length
            ? this._tokens[this.Position + offset]
            : this._tokens[this._tokens.Length - 1];


        /// <inheritdoc />
        [SuppressMessage ( "Style", "IDE0056:Use index operator", Justification = "Not available on all target frameworks." )]
        [SuppressMessage ( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Valid for some target frameworks." )]
        public Token<TTokenType> Consume ( )
        {
            if ( this.Position >= this.Length )
                return this._tokens[this._tokens.Length - 1];

            Token<TTokenType> token = this._tokens[this.Position];
            this.Position++;
            return token;
        }

        /// <inheritdoc />
        public void Skip ( Int32 count ) =>
            this.Position += System.Math.Min ( count, this.Length - this.Position );

        #region IsAhead

        /// <inheritdoc />
        public Boolean IsAhead ( TTokenType tokenType, Int32 offset = 0 ) =>
            EqualityComparer<TTokenType>.Default.Equals ( this.Lookahead ( offset ).Type, tokenType );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TTokenType> tokenTypes, Int32 offset = 0 )
        {
            if ( tokenTypes is null )
                throw new ArgumentNullException ( nameof ( tokenTypes ) );

            return tokenTypes.Contains ( this.Lookahead ( offset ).Type );
        }

        /// <inheritdoc />
        public Boolean IsAhead ( String id, Int32 offset = 0 ) =>
            StringComparer.Ordinal.Equals ( this.Lookahead ( offset ).Id, id );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 )
        {
            if ( ids is null )
                throw new ArgumentNullException ( nameof ( ids ) );

            return ids.Contains ( this.Lookahead ( offset ).Id, StringComparer.Ordinal );
        }

        /// <inheritdoc />
        public Boolean IsAhead ( TTokenType tokenType, String id, Int32 offset = 0 )
        {
            Token<TTokenType> ahead = this.Lookahead ( offset );
            return EqualityComparer<TTokenType>.Default.Equals ( tokenType, ahead.Type )
                   && StringComparer.Ordinal.Equals ( id, ahead.Id );
        }

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TTokenType> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 )
        {
            if ( tokenTypes is null )
                throw new ArgumentNullException ( nameof ( tokenTypes ) );
            if ( ids is null )
                throw new ArgumentNullException ( nameof ( ids ) );

            Token<TTokenType> ahead = this.Lookahead ( offset );
            return tokenTypes.Contains ( ahead.Type ) && ids.Contains ( ahead.Id, StringComparer.Ordinal );
        }

        #endregion IsAhead

        #region Accept

        /// <inheritdoc />
        public Boolean Accept ( String id, [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( id ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( IEnumerable<String> ids, [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( ids ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( TTokenType type, [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( type ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( IEnumerable<TTokenType> types, [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( types ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept ( TTokenType type, String id, [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( type, id ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        /// <inheritdoc />
        public Boolean Accept (
            IEnumerable<TTokenType> types,
            IEnumerable<String> ids,
            [NotNullWhen ( true )] out Token<TTokenType>? token )
        {
            if ( this.IsAhead ( types, ids ) )
            {
                token = this.Consume ( );
                return true;
            }
            token = default;
            return false;
        }

        #endregion Accept

        #endregion ITokenReader<TTokenType>

        /// <inheritdoc/>
        public void Restore ( Int32 position )
        {
            if ( position < 0 )
                throw new ArgumentOutOfRangeException ( nameof ( position ), "The position must be positive." );
            if ( position >= this.Length )
                throw new ArgumentOutOfRangeException ( nameof ( position ), "The position must be less than the token list length." );
            this.Position = position;
        }
    }
}
