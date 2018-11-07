using System;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Abstractions.Parsing.Modules;

namespace GParse.Parsing.Abstractions.Parsing
{
    public interface IPrattParserBuilder<TokenTypeT, ExpressionNodeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers a prefix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="prefixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IPrefixModule<TokenTypeT, ExpressionNodeT> prefixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Registers an infix expression parser module
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="ID"></param>
        /// <param name="infixModule"></param>
        void Register ( TokenTypeT tokenType, String ID, IInfixModule<TokenTypeT, ExpressionNodeT> infixModule );

        /// <summary>
        /// Initializes a new Pratt Parser
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        IPrattParser<TokenTypeT, ExpressionNodeT> CreateParser ( ITokenReader<TokenTypeT> reader );
    }
}
