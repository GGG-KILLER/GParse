using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;

namespace GParse.Verbose.AST
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
