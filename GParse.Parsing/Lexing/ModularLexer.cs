using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Lexing.Errors;

namespace GParse.Parsing.Lexing
{
    public class ModularLexer<TokenTypeT> : ILexer<TokenTypeT> where TokenTypeT : Enum
    {
        private readonly LexerModuleTree<TokenTypeT> ModuleTree;
        private readonly SourceCodeReader Reader;
        private Boolean EOFReturned;

        internal ModularLexer ( LexerModuleTree<TokenTypeT> tree, SourceCodeReader reader )
        {
            this.ModuleTree = tree;
            this.Reader = reader;
        }

        protected virtual Token<TokenTypeT> InternalConsumeToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try
            {
                if ( this.Reader.IsAtEOF )
                {
                    if ( this.EOFReturned )
                        throw new UnableToContinueLexingException ( loc, "The lexer has hit the end of the file, there are no contents to tokenize left.", this.Reader );

                    this.EOFReturned = true;
                    return new Token<TokenTypeT> ( "EOF", "", "", default, this.Reader.Location.To ( this.Reader.Location ) );
                }

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

                throw new UnableToContinueLexingException ( this.Reader.Location, $"Unable to parse anything past this point.", this.Reader );
            }
            catch
            {
                this.Reader.Rewind ( loc );
                throw;
            }
        }

        protected virtual Token<TokenTypeT> GetFirstMeaningfulToken ( )
        {
            Token<TokenTypeT> tok;
            var triviaAccumulator = new List<Token<TokenTypeT>> ( );
            while ( ( tok = this.InternalConsumeToken ( ) ).IsTrivia )
                triviaAccumulator.Add ( tok );

            return new Token<TokenTypeT> ( tok.ID, tok.Raw, tok.Value, tok.Type, tok.Range, false, triviaAccumulator.ToArray ( ) );
        }

        #region Lexer

        public Token<TokenTypeT> ConsumeToken ( ) => this.GetFirstMeaningfulToken ( );

        public Boolean AcceptToken ( String ID, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.ConsumeToken ( );
            if ( token?.ID != ID )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( String ID ) => this.AcceptToken ( ID, out _ );

        public Boolean AcceptToken ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.ConsumeToken ( );
            if ( !token.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( TokenTypeT type ) => this.AcceptToken ( type, out _ );

        public Boolean AcceptToken ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.ConsumeToken ( );
            if ( token == null || token.ID != ID || !token.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( String ID, TokenTypeT type ) => this.AcceptToken ( ID, type, out _ );

        #region IReadOnlyLexer

        public Boolean EOF => this.Reader.IsAtEOF;

        public SourceLocation Location => this.Reader.Location;

        public Int32 ContentLeft => this.Reader.ContentLeft;

        public Token<TokenTypeT> PeekToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.ConsumeToken ( ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        public Boolean IsNextToken ( String ID, out Token<TokenTypeT> token )
        {
            token = this.PeekToken ( );
            return token != null && token.ID == ID;
        }

        public Boolean IsNextToken ( String ID ) => this.IsNextToken ( ID, out _ );

        public Boolean IsNextToken ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            token = this.PeekToken ( );
            return token != null && token.Type.Equals ( type );
        }

        public Boolean IsNextToken ( TokenTypeT type ) => this.IsNextToken ( type, out _ );

        public Boolean IsNextToken ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            token = this.PeekToken ( );
            return token != null && token.ID == ID && token.Type.Equals ( type );
        }

        public Boolean IsNextToken ( String ID, TokenTypeT type ) => this.IsNextToken ( ID, type, out _ );

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
