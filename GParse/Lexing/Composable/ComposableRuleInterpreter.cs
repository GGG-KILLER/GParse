using System;
using System.Collections.Generic;
using System.Text;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Modules;

namespace GParse.Lexing.Composable
{
    internal class ComposableRuleInterpreter<TTokenType> : ILexerModule<TTokenType>
    {
        public String Name { get; }
        public String Prefix { get; }
        private GrammarNode<Char> RootNode { get; }


        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader )
        {
            throw new NotImplementedException ( );
        }

        public Token<TTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            throw new NotImplementedException ( );
        }
    }
}
