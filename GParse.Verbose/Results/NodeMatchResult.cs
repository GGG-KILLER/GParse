using System;
using GParse.Common;

namespace GParse.Verbose.Results
{
    internal class NodeMatchResult : MatchResult
    {
        public readonly ASTNode Node;

        public NodeMatchResult ( SourceRange range, ASTNode node, String name = null ) : base ( range, name )
        {
            this.Node = node;
        }
    }
}
