using System;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Abstractions.Parsing
{
    public interface IParserBuilder<TokenTypeT> where TokenTypeT : IEquatable<TokenTypeT>
    {
        void AddModule ( IParserModule<TokenTypeT> module );

        IParser BuildParser ( ILexer<TokenTypeT> lexer );
    }
}
