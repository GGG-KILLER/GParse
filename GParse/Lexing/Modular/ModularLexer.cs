using System;
using GParse.Errors;
using GParse.IO;
using GParse.Lexing.Modular;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A modular lexer created by the <see cref="ModularLexerBuilder{TokenTypeT}" />
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class ModularLexer<TokenTypeT> : BaseLexer<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// This lexer's module tree
        /// </summary>
        protected readonly LexerModuleTree<TokenTypeT> _moduleTree;

        /// <summary>
        /// The toke
        /// </summary>
        private readonly TokenTypeT _eofTokenType;

        /// <summary>
        /// Initializes a new lexer
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="eofTokenType"></param>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        protected internal ModularLexer (
            LexerModuleTree<TokenTypeT> tree,
            TokenTypeT eofTokenType,
            ICodeReader reader,
            DiagnosticList diagnostics )
            : base ( reader, diagnostics )
        {
            this._moduleTree = tree ?? throw new ArgumentNullException ( nameof ( tree ) );
            this._eofTokenType = eofTokenType;
        }

        /// <summary>
        /// Consumes a token accounting for trivia tokens.
        /// </summary>
        /// <returns></returns>
        protected override Token<TokenTypeT> GetNextToken ( )
        {
            ICodeReader reader = this._reader;
            var start = reader.Position;
            try
            {
                if ( this.EndOfFile )
                    return new Token<TokenTypeT> ( "EOF", this._eofTokenType, new Range<Int32> ( reader.Position ) );

                foreach ( ILexerModule<TokenTypeT> module in this._moduleTree.GetSortedCandidates ( reader ) )
                {
                    if ( module.TryConsume ( reader, this._diagnostics, out Token<TokenTypeT> token ) )
                    {
                        return token;
                    }

                    if ( reader.Position != start )
                    {
                        SourceLocation startPosition = reader.GetLocation ( start );
                        throw new FatalParsingException ( startPosition.To ( reader.GetLocation ( ) ), $"Lexing module '{module.Name}' modified state on CanConsumeNext and did not restore it." );
                    }
                }

                throw new FatalParsingException ( reader.GetLocation ( ), $"No registered modules can consume the rest of the input." );
            }
            catch
            {
                this._reader.Restore ( start );
                throw;
            }
        }
    }
}
