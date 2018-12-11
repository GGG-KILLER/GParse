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
    /// <summary>
    /// A modular lexer created by the <see cref="ModularLexerBuilder{TokenTypeT}" />
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class ModularLexer<TokenTypeT> : ILexer<TokenTypeT>
    {
        /// <summary>
        /// This lexer's module tree
        /// </summary>
        protected readonly LexerModuleTree<TokenTypeT> ModuleTree;

        /// <summary>
        /// The reader being used by the lexer
        /// </summary>
        protected readonly SourceCodeReader Reader;

        /// <summary>
        /// Initializes a new lexer
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="reader"></param>
        protected internal ModularLexer ( LexerModuleTree<TokenTypeT> tree, SourceCodeReader reader )
        {
            this.ModuleTree = tree;
            this.Reader = reader;
        }

        /// <summary>
        /// Consumes a token
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves the first meaningful token while
        /// accumulating trivia
        /// </summary>
        /// <returns></returns>
        protected virtual Token<TokenTypeT> GetFirstMeaningfulToken ( )
        {
            Token<TokenTypeT> tok;
            var triviaAccumulator = new List<Token<TokenTypeT>> ( );
            while ( ( tok = this.InternalConsumeToken ( ) ).IsTrivia )
                triviaAccumulator.Add ( tok );

            return new Token<TokenTypeT> ( tok.ID, tok.Raw, tok.Value, tok.Type, tok.Range, false, triviaAccumulator.ToArray ( ) );
        }

        #region Lexer

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public Token<TokenTypeT> Consume ( ) => this.GetFirstMeaningfulToken ( );

        #region IReadOnlyLexer

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Boolean EOF => this.Reader.IsAtEOF;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public SourceLocation Location => this.Reader.Location;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Int32 ContentLeft => this.Reader.ContentLeft;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public Token<TokenTypeT> Peek ( )
        {
            SourceLocation loc = this.Reader.Location;
            try { return this.Consume ( ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        #endregion IReadOnlyLexer

        #endregion Lexer
    }
}
