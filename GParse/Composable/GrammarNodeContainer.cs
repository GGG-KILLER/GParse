using System;

namespace GParse.Composable
{
    /// <summary>
    /// The base class for nodes that wrap other nodes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GrammarNodeContainer<T> : GrammarNode<T>
    {
        /// <summary>
        /// The inner node.
        /// </summary>
        public GrammarNode<T> InnerNode { get; }

        /// <summary>
        /// Initializes a new node container with the provided node.
        /// </summary>
        /// <param name="node"></param>
        protected GrammarNodeContainer ( GrammarNode<T> node )
        {
            this.InnerNode = node ?? throw new ArgumentNullException ( nameof ( node ) );
        }
    }
}
