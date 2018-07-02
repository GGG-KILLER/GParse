using System;
using GParse.Common.AST;

namespace GParse.Verbose.Parsing.AST

{
    public class MarkerNode : ASTNode
    {
        public readonly String Content;

        public MarkerNode ( String contents )
        {
            if ( String.IsNullOrEmpty ( contents ) )
                throw new ArgumentException ( "Contents can't be null or empty.", nameof ( contents ) );

            this.Content = contents;
        }
    }
}
