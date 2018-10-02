using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Parsing
{
    public abstract class HandwrittenParserBase<TokenTypeT> where TokenTypeT : Enum
    {
        protected readonly ILexer<TokenTypeT> Lexer;
        private Token<TokenTypeT> TokenCache;

        protected HandwrittenParserBase ( ILexer<TokenTypeT> lexer )
        {
            this.Lexer = lexer;
        }

        protected Token<TokenTypeT> PeekToken ( )
        {
            if ( this.TokenCache == null )
                this.TokenCache = this.Lexer.ConsumeToken ( );
            return this.TokenCache;
        }

        protected Token<TokenTypeT> ReadToken ( )
        {
            try
            {
                return this.PeekToken ( );
            }
            finally
            {
                this.TokenCache = null;
            }
        }

        #region Consume

        protected Boolean Consume ( String ID, out Token<TokenTypeT> token )
        {
            if ( this.PeekToken ( ).ID == ID )
            {
                token = this.ReadToken ( );
                return true;
            }
            token = null;
            return false;
        }

        protected Boolean Consume ( String ID ) => this.Consume ( ID, out _ );

        protected Boolean Consume ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.PeekToken ( ).Type.Equals ( type ) )
            {
                token = this.ReadToken ( );
                return true;
            }
            token = null;
            return false;
        }

        protected Boolean Consume ( TokenTypeT type ) => this.Consume ( type, out _ );

        protected Boolean Consume ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            if ( this.PeekToken ( ).Type.Equals ( type ) && this.PeekToken ( ).ID == ID )
            {
                token = this.ReadToken ( );
                return true;
            }
            token = null;
            return false;
        }

        protected Boolean Consume ( String ID, TokenTypeT type ) => this.Consume ( ID, type, out _ );

        #endregion Consume

        #region Expect

        protected Token<TokenTypeT> Expect ( String ID )
        {
            Token<TokenTypeT> peek = this.PeekToken ( );
            if ( peek == null || peek.ID != ID )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID} but got {peek?.ID ?? "EOF"} instead." );
            return this.ReadToken ( );
        }

        protected Token<TokenTypeT> Expect ( TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.PeekToken ( );
            if ( peek == null || !peek.Type.Equals ( type ) )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {type} but got {peek?.Type.ToString ( ) ?? "EOF"} instead." );
            return this.ReadToken ( );
        }

        protected Token<TokenTypeT> Expect ( String ID, TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.PeekToken ( );
            if ( peek == null || !peek.Type.Equals ( type ) || peek.ID != ID )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID}+{type} but got a {peek?.ID ?? "EOF"}+{peek?.Type.ToString ( ) ?? "EOF"}" );
            return this.ReadToken ( );
        }

        #endregion Expect
    }
}
