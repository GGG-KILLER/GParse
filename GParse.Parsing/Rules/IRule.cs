using System;
using GParse.Lexing;

namespace GParse.Parsing.Rules
{
    internal interface IRule
    {
        Boolean Matches ( LexerBase lexer );
    }
}
