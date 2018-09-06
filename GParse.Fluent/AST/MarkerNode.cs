using System;
using System.Collections.Generic;
using GParse.Common.AST;

namespace GParse.Fluent.AST

{
    public class MarkerNode : ASTNode, IEquatable<MarkerNode>
    {
        public readonly String Content;

        public MarkerNode ( String contents )
        {
            this.Content = contents ?? throw new ArgumentNullException ( nameof ( contents ) );
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as MarkerNode );
        }

        public Boolean Equals ( MarkerNode other )
        {
            return other != null &&
                     this.Content == other.Content;
        }

        public override Int32 GetHashCode ( )
        {
            return 1997410482 + EqualityComparer<String>.Default.GetHashCode ( this.Content );
        }

        public static Boolean operator == ( MarkerNode node1, MarkerNode node2 ) => EqualityComparer<MarkerNode>.Default.Equals ( node1, node2 );

        public static Boolean operator != ( MarkerNode node1, MarkerNode node2 ) => !( node1 == node2 );

        #endregion Generated Code
    }
}
