using System;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public class LexerBuilder<TokenTypeT> : ILexerBuilder<TokenTypeT> where TokenTypeT : Enum
    {
        // Root of the tree is always null, since we don't have
        // common prefixes in the general case.
        private readonly LexerModuleTree<TokenTypeT> Modules = new LexerModuleTree<TokenTypeT> ( );

        public void AddModule ( ILexerModule<TokenTypeT> module ) => this.Modules.AddChild ( module );

        public ILexer<TokenTypeT> BuildLexer ( String input ) => this.BuildLexer ( new SourceCodeReader ( input ) );

        public ILexer<TokenTypeT> BuildLexer ( SourceCodeReader reader ) => new ModularLexer<TokenTypeT> ( this.Modules, reader );
    }
}
