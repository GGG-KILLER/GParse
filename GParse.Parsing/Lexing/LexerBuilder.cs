using System;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    public class LexerBuilder : ILexerBuilder
    {
        // Root of the tree is always null, since we don't have
        // common prefixes in the general case.
        private readonly LexerModuleTree Modules = new LexerModuleTree ( );

        public void AddModule ( ILexerModule module ) => this.Modules.AddChild ( module );

        public ILexer BuildLexer ( String input ) => this.BuildLexer ( new SourceCodeReader ( input ) );

        public ILexer BuildLexer ( SourceCodeReader reader ) => new ModularLexer ( this.Modules, reader );
    }
}
