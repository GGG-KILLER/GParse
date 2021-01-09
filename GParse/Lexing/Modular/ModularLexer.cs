using System;
using System.Runtime.CompilerServices;
using GParse.Errors;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A modular lexer created by the <see cref="ModularLexerBuilder{TTokenType}" />
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class ModularLexer<TTokenType> : BaseLexer<TTokenType>
        where TTokenType : notnull
    {

        /// <summary>
        /// This lexer's module tree
        /// </summary>
        protected LexerModuleTree<TTokenType> ModuleTree { get; }

        /// <summary>
        /// The token type to use when emitting an EOF token.
        /// </summary>
        protected TTokenType EndOfFileTokenType { get; }

        /// <summary>
        /// Initializes a new lexer.
        /// You can call this directly but you shouldn't.
        /// Call <see cref="ModularLexerBuilder{TTokenType}.GetLexer(ICodeReader, DiagnosticList)"/> instead.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="endOfFileTokenType"></param>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        public ModularLexer (
            LexerModuleTree<TTokenType> tree,
            TTokenType endOfFileTokenType,
            ICodeReader reader,
            DiagnosticList diagnostics )
            : base ( reader, diagnostics )
        {
            this.ModuleTree = tree ?? throw new ArgumentNullException ( nameof ( tree ) );
            this.EndOfFileTokenType = endOfFileTokenType;
        }

        /// <summary>
        /// Consumes a token accounting for trivia tokens.
        /// </summary>
        /// <returns></returns>
        protected override Token<TTokenType> GetNextToken ( )
        {
            ICodeReader reader = this.Reader;
            var start = reader.Position;
            try
            {
                if ( this.EndOfFile )
                    return new Token<TTokenType> ( "EOF", this.EndOfFileTokenType, new Range<Int32> ( reader.Position ) );

                foreach ( ILexerModule<TTokenType> module in this.ModuleTree.GetSortedCandidates ( reader ) )
                {
                    if ( module.TryConsume ( reader, this.Diagnostics, out Token<TTokenType>? token ) )
                    {
                        return token;
                    }

                    if ( reader.Position != start )
                    {
                        reader.Restore ( start );
                    }
                }

                if ( this.ModuleTree.FallbackModule is ILexerModule<TTokenType> fallbackModule && fallbackModule.TryConsume ( reader, this.Diagnostics, out Token<TTokenType>? fallbackToken ) )
                    return fallbackToken;
                throw new FatalParsingException ( reader.GetLocation ( ), $"No registered modules can consume the rest of the input." );
            }
            catch when ( restore ( reader, start ) )
            {
                throw;
            }

            // We use this method so that when an exception is thrown, we don't
            // rewind the stack but still restore the reader's position.
            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Boolean restore ( ICodeReader reader, Int32 position )
            {
                reader.Restore ( position );
                return false;
            }
        }
    }
}
