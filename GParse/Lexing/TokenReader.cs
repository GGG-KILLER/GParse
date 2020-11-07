using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using GParse.Errors;

namespace GParse.Lexing
{
    /// <summary>
    /// A token reader that reads tokens as requested and caches them
    /// when lookaheads are requested.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class TokenReader<TokenTypeT> : ITokenReader<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly ILexer<TokenTypeT> _lexer;
        private readonly ImmutableArray<Token<TokenTypeT>> _tokens;

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
        public TokenReader ( ILexer<TokenTypeT> lexer )
        {
            this.Position = 0;
            ImmutableArray<Token<TokenTypeT>>.Builder tokens = ImmutableArray.CreateBuilder<Token<TokenTypeT>> ( );
            while ( !lexer.EndOfFile )
                tokens.Add ( lexer.Consume ( ) );
            this._tokens = tokens.ToImmutable ( );
            this._lexer = lexer;
        }

        #region ITokenReader<TokenTypeT>

        /// <inheritdoc />
        [SuppressMessage ( "Style", "IDE0056:Use index operator", Justification = "Not available on all target frameworks." )]
        public Token<TokenTypeT> Lookahead ( Int32 offset = 0 ) =>
            this.Position + offset < this.Length
            ? this._tokens[this.Position + offset]
            : this._tokens[this._tokens.Length - 1];


        /// <inheritdoc />
        [SuppressMessage ( "Style", "IDE0056:Use index operator", Justification = "Not available on all target frameworks." )]
        public Token<TokenTypeT> Consume ( )
        {
            if ( this.Position >= this.Length )
                return this._tokens[this._tokens.Length - 1];

            Token<TokenTypeT> token = this._tokens[this.Position];
            this.Position++;
            return token;
        }

        /// <inheritdoc />
        public void Skip ( Int32 count ) =>
            this.Position += System.Math.Min ( count, this.Length - this.Position );

        #region IsAhead

        /// <inheritdoc />
        public Boolean IsAhead ( TokenTypeT tokenType, Int32 offset = 0 ) =>
            EqualityComparer<TokenTypeT>.Default.Equals ( this.Lookahead ( offset ).Type, tokenType );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, Int32 offset = 0 )
        {
            TokenTypeT type = this.Lookahead ( offset ).Type;
            foreach ( TokenTypeT wtype in tokenTypes )
            {
                if ( EqualityComparer<TokenTypeT>.Default.Equals ( wtype, type ) )
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public Boolean IsAhead ( String ID, Int32 offset = 0 ) =>
            this.Lookahead ( offset ).Id == ID;

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 )
        {
            var aheadId = this.Lookahead ( offset ).Id;
            foreach ( var id in ids )
            {
                if ( id == aheadId )
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public Boolean IsAhead ( TokenTypeT tokenType, String id, Int32 offset = 0 ) =>
            this.IsAhead ( tokenType, offset ) && this.IsAhead ( id, offset );

        /// <inheritdoc />
        public Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 ) =>
            this.IsAhead ( tokenTypes, offset ) && this.IsAhead ( ids, offset );

        #endregion IsAhead

        #region Accept

        /// <inheritdoc />
        public Boolean Accept ( String id, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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
        public Boolean Accept ( IEnumerable<String> ids, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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
        public Boolean Accept ( TokenTypeT type, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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
        public Boolean Accept ( IEnumerable<TokenTypeT> types, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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
        public Boolean Accept ( TokenTypeT type, String id, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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
            IEnumerable<TokenTypeT> types,
            IEnumerable<String> ids,
            [NotNullWhen ( true )] out Token<TokenTypeT>? token )
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

        #region FatalExpect

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( String id )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( id, out _ ) )
            {
                throw new FatalParsingException (
                    this._lexer.GetLocation ( next.Range ),
                    $"Expected a {id} but got {next.Id} instead." );
            }

            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<String> ids )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( ids, out _ ) )
            {
                throw new FatalParsingException (
                    this._lexer.GetLocation ( next.Range ),
                    $"Expected any ({String.Join ( ", ", ids )}) but got {next.Id}" );
            }
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( TokenTypeT type )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( type, out _ ) )
            {
                throw new FatalParsingException (
                    this._lexer.GetLocation ( next.Range ),
                    $"Expected a {type} but got {next.Type} instead." );
            }

            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( types, out _ ) )
                throw new FatalParsingException ( this._lexer.GetLocation ( next.Range ), $"Expected any ({String.Join ( ", ", types )}) but got {next.Type}" );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( TokenTypeT type, String ID )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( type, ID, out _ ) )
                throw new FatalParsingException ( this._lexer.GetLocation ( next.Range ), $"Expected a {ID}+{type} but got a {next.Id}+{next.Type}" );
            return next;
        }

        /// <inheritdoc />
        public Token<TokenTypeT> FatalExpect ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs )
        {
            Token<TokenTypeT> next = this.Lookahead ( );
            if ( !this.Accept ( types, IDs, out _ ) )
                throw new FatalParsingException ( this._lexer.GetLocation ( next.Range ), $"Expected any ({String.Join ( ", ", IDs )})+({String.Join ( ", ", types )}) but got {next.Id}+{next.Type}" );
            return next;
        }

        #endregion FatalExpect

        #endregion ITokenReader<TokenTypeT>

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
