using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Parsing
{
    public class HandwrittenParserBase
    {
        protected readonly ILexer Lexer;
        private Token TokenCache;

        protected HandwrittenParserBase ( ILexer lexer )
        {
            this.Lexer = lexer;
        }

        protected Token PeekToken ( )
        {
            if ( this.TokenCache == null )
                this.TokenCache = this.Lexer.ConsumeToken ( );
            return this.TokenCache;
        }

        protected Token ReadToken ( )
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

        protected Boolean Consume ( String ID, out Token token )
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

        protected Boolean Consume ( TokenType type, out Token token )
        {
            if ( this.PeekToken ( ).Type == type )
            {
                token = this.ReadToken ( );
                return true;
            }
            token = null;
            return false;
        }

        protected Boolean Consume ( TokenType type ) => this.Consume ( type, out _ );

        protected Boolean Consume ( String ID, TokenType type, out Token token )
        {
            if ( this.PeekToken ( ).ID == ID && this.PeekToken ( ).Type == type )
            {
                token = this.ReadToken ( );
                return true;
            }
            token = null;
            return false;
        }

        protected Boolean Consume ( String ID, TokenType type ) => this.Consume ( ID, type, out _ );

        #endregion Consume

        #region Expect

        protected Token Expect ( String ID )
        {
            Token peek = this.PeekToken ( );
            if ( peek == null || peek.ID != ID )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID} but got {peek?.ID ?? "EOF"} instead." );
            return this.ReadToken ( );
        }

        protected Token Expect ( TokenType type )
        {
            Token peek = this.PeekToken ( );
            if ( peek == null || peek.Type != type )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {type} but got {peek?.Type.ToString ( ) ?? "EOF"} instead." );
            return this.ReadToken ( );
        }

        protected Token Expect ( String ID, TokenType type )
        {
            Token peek = this.PeekToken ( );
            if ( peek == null || peek.ID != ID || peek.Type == type )
                throw new ParseException ( peek?.Range.Start ?? SourceLocation.Min, $"Expected a {ID}+{type} but got a {peek?.ID ?? "EOF"}+{peek?.Type.ToString ( ) ?? "EOF"}" );
            return this.ReadToken ( );
        }

        #endregion Expect
    }
}
