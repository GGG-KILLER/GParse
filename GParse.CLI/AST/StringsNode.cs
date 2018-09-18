using System;
using GParse.Common.AST;

namespace GParse.CLI.AST
{
    internal class StringNode : ASTNode
    {
        public readonly StringNode[] Children;
        public readonly String Value;

        public StringNode ( StringNode[] children, String value )
        {
            this.Children = children;
            this.Value = value;
        }

        public override String ToString ( )
        {
            return this.Children.Length == 0
                ? $"{{{this.Value}}}"
                : $"{{{this.Value}, {{{String.Join<StringNode> ( ", ", this.Children )}}}}}";
        }
    }
}
