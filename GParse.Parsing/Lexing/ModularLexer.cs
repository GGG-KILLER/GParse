using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public class ModularLexer<TokenTypeT> : ILexer<TokenTypeT> where TokenTypeT : Enum
    {
        private readonly LexerModuleTree<TokenTypeT> ModuleTree;
        private readonly SourceCodeReader Reader;

        internal ModularLexer ( LexerModuleTree<TokenTypeT> tree, SourceCodeReader reader )
        {
            this.ModuleTree = tree;
            this.Reader = reader;
        }

        #region Lexer

        public Token<TokenTypeT> ConsumeToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try
            {
                if ( this.Reader.IsAtEOF )
                    return new Token<TokenTypeT> ( "EOF", "", "", default, this.Reader.Location.To ( this.Reader.Location ) );

                foreach ( ILexerModule<TokenTypeT> module in this.ModuleTree.GetSortedCandidates ( this.Reader ) )
                {
                    if ( module.CanConsumeNext ( this.Reader ) )
                    {
                        try
                        {
                            return module.ConsumeNext ( this.Reader );
                        }
                        catch ( Exception ex )
                        {
                            throw new LexingException ( loc, ex.Message, ex );
                        }
                    }
                    if ( this.Reader.Location != loc )
                        throw new LexingException ( loc, $"Lexing module '{module.Name}' modified state on CanConsumeNext and did not restore it." );
                }

                throw new LexingException ( this.Reader.Location, $"Unable to parse anything past this point:\n{this.Reader}" );
            }
            catch
            {
                this.Reader.Rewind ( loc );
                throw;
            }
        }

        public Boolean AcceptToken ( String ID )
        {
            SourceLocation loc = this.Reader.Location;
            Token<TokenTypeT> tok = this.ConsumeToken ( );
            if ( tok?.ID != ID )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( TokenTypeT type )
        {
            SourceLocation loc = this.Reader.Location;
            Token<TokenTypeT> tok = this.ConsumeToken ( );
            if ( tok == null || !tok.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( String ID, TokenTypeT type )
        {
            SourceLocation loc = this.Reader.Location;
            Token<TokenTypeT> tok = this.ConsumeToken ( );
            if ( tok == null || tok.ID != ID || !tok.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        #region IReadOnlyLexer

        public Boolean EOF => this.Reader.IsAtEOF;

        public SourceLocation Location => this.Reader.Location;

        public Int32 ContentLeft => this.Reader.ContentLeftSize;

        public Token<TokenTypeT> PeekToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.ConsumeToken ( ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        public Boolean IsNextToken ( String ID )
        {
            Token<TokenTypeT> peek = this.PeekToken ( );
            return peek != null && peek.ID == ID;
        }

        public Boolean IsNextToken ( TokenTypeT type )
        {
            Token<TokenTypeT> peek = this.PeekToken ( );
            return peek != null && peek.Type.Equals ( type );
        }

        public Boolean IsNextToken ( String ID, TokenTypeT type )
        {
            Token<TokenTypeT> tok = this.PeekToken ( );
            return tok != null && tok.Type.Equals ( type ) && tok.ID == ID;
        }

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
