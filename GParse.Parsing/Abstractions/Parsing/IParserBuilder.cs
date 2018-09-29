using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Abstractions.Parsing
{
    public interface IParserBuilder
    {
        void AddModule ( IParserModule module );

        IParser BuildParser ( ILexer lexer );
    }
}
