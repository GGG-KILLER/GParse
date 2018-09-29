using System;
using GParse.Common;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public class ModularLexer : ILexer
    {
        private readonly LexerModuleTree ModuleTree;
        private readonly SourceCodeReader Reader;

        internal ModularLexer ( LexerModuleTree tree, SourceCodeReader reader )
        {
            this.ModuleTree = tree;
            this.Reader = reader;
        }

        #region Lexer

        public Token ConsumeToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try
            {
                foreach ( ILexerModule module in this.ModuleTree.GetSortedCandidates ( this.Reader ) )
                    if ( module.CanConsumeNext ( this.Reader ) )
                        return module.ConsumeNext ( this.Reader );
                return null;
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
            Token tok = this.ConsumeToken ( );
            if ( tok?.ID != ID )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( TokenType type )
        {
            SourceLocation loc = this.Reader.Location;
            Token tok = this.ConsumeToken ( );
            if ( tok?.Type != type )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        public Boolean AcceptToken ( String ID, TokenType type )
        {
            SourceLocation loc = this.Reader.Location;
            Token tok = this.ConsumeToken ( );
            if ( tok?.ID != ID || tok?.Type != type )
            {
                this.Reader.Rewind ( loc );
                return false;
            }

            return true;
        }

        #region IReadOnlyLexer

        public Boolean EOF => this.Reader.IsAtEOF;

        public Token PeekToken ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.ConsumeToken ( ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        public Boolean IsNextToken ( String ID ) => this.PeekToken ( )?.ID == ID;

        public Boolean IsNextToken ( TokenType type ) => this.PeekToken ( )?.Type == type;

        public Boolean IsNextToken ( String ID, TokenType type )
        {
            Token tok = this.PeekToken ( );
            return tok?.ID == ID && tok?.Type == type;
        }

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
