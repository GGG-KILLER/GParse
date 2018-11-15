using System;
using System.Collections.Generic;
using GParse.Common.AST;

namespace GParse.Fluent.AST
{
    /// <summary>
    /// Defines a marker node
    /// </summary>
    public class MarkerNode : ASTNode, IEquatable<MarkerNode>
    {
        /// <summary>
        /// The contents of the marker node
        /// </summary>
        public readonly String Content;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="contents"></param>
        public MarkerNode ( String contents )
        {
            this.Content = contents ?? throw new ArgumentNullException ( nameof ( contents ) );
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => this.Equals ( obj as MarkerNode );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( MarkerNode other ) => other != null &&
                     this.Content == other.Content;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( ) => 1997410482 + EqualityComparer<String>.Default.GetHashCode ( this.Content );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public static Boolean operator == ( MarkerNode node1, MarkerNode node2 ) => EqualityComparer<MarkerNode>.Default.Equals ( node1, node2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public static Boolean operator != ( MarkerNode node1, MarkerNode node2 ) => !( node1 == node2 );

        #endregion Generated Code
    }
}
