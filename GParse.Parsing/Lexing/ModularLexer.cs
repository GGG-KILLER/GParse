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

        public Token<TokenTypeT> Consume ( ) => this.GetFirstMeaningfulToken ( );

        public Boolean Accept ( String ID, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.Consume ( );
            if ( token?.ID != ID )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean Accept ( String ID ) => this.Accept ( ID, out _ );

        public Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.Consume ( );
            if ( !token.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean Accept ( TokenTypeT type ) => this.Accept ( type, out _ );

        public Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            SourceLocation loc = this.Reader.Location;
            token = this.Consume ( );
            if ( token == null || token.ID != ID || !token.Type.Equals ( type ) )
            {
                this.Reader.Rewind ( loc );
                token = null;
                return false;
            }

            return true;
        }

        public Boolean Accept ( String ID, TokenTypeT type ) => this.Accept ( ID, type, out _ );

        #region IReadOnlyLexer

        public Boolean EOF => this.Reader.IsAtEOF;

        public SourceLocation Location => this.Reader.Location;

        public Int32 ContentLeft => this.Reader.ContentLeft;

        public Token<TokenTypeT> Peek ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.Consume ( ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        public Boolean IsNext ( String ID, out Token<TokenTypeT> token )
        {
            token = this.Peek ( );
            return token != null && token.ID == ID;
        }

        public Boolean IsNext ( String ID ) => this.IsNext ( ID, out _ );

        public Boolean IsNext ( TokenTypeT type, out Token<TokenTypeT> token )
        {
            token = this.Peek ( );
            return token != null && token.Type.Equals ( type );
        }

        public Boolean IsNext ( TokenTypeT type ) => this.IsNext ( type, out _ );

        public Boolean IsNext ( String ID, TokenTypeT type, out Token<TokenTypeT> token )
        {
            token = this.Peek ( );
            return token != null && token.ID == ID && token.Type.Equals ( type );
        }

        public Boolean IsNext ( String ID, TokenTypeT type ) => this.IsNext ( ID, type, out _ );

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
