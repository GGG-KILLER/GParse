using GParse.Common.AST;

namespace GParse.Parsing.Abstractions.Parsing
{
    public interface IParser
    {
        ASTNode Parse ( );
    }
}
