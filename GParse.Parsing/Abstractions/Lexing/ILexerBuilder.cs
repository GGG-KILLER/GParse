using System;
using GParse.Common.IO;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexerBuilder<TokenTypeT> where TokenTypeT : IEquatable<TokenTypeT>
    {
        void AddModule ( ILexerModule<TokenTypeT> module );

        ILexer<TokenTypeT> BuildLexer ( String input );

        ILexer<TokenTypeT> BuildLexer ( SourceCodeReader reader );
    }
}
