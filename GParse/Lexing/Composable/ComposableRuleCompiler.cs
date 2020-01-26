using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using GParse.Composable;
using GParse.Lexing.Modules;

namespace GParse.Lexing.Composable
{
    public class ComposableRuleCompiler<TTokenType> : IComposableRuleCompiler<TTokenType>
    {
        public ILexerModule<TTokenType> CompileRule ( GrammarNode<Char> grammarNode )
        {
            throw new NotImplementedException ( );
        }
    }
}
