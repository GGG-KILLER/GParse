using System;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal abstract class Node
    {
        public Boolean IsLazy { get; internal set; }

        public abstract T Accept<T> ( ITreeVisitor<T> visitor );

        public override String ToString ( ) => this.Accept ( new RegexWriter ( ) );
    }
}
