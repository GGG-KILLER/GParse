using System;
using GParse.Common.IO;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexerBuilder
    {
        void AddModule ( ILexerModule module );

        ILexer BuildLexer ( String input );

        ILexer BuildLexer ( SourceCodeReader reader );
    }
}
