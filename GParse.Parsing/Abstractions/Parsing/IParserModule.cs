using System;
using GParse.Common.AST;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Abstractions.Parsing
{
    public interface IParserModule<TokenTypeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Whether this module can consume what's left in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Boolean CanConsumeNext ( IReadOnlyLexer<TokenTypeT> lexer );

        /// <summary>
        /// Consume the next element in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        ASTNode ConsumeNext ( ILexer<TokenTypeT> lexer );
    }
}
