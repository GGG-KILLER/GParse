using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a node that contains other nodes as it's children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GrammarNodeListContainer<T> : GrammarNode
        where T : GrammarNodeListContainer<T>
    {
        /// <summary>
        /// The list of grammar nodes
        /// </summary>
        protected readonly List<GrammarNode> grammarNodes;

        /// <summary>
        /// Initializes a new <see cref="GrammarNodeListContainer{T}" />
        /// </summary>
        /// <param name="grammarNodes"></param>
        protected GrammarNodeListContainer ( GrammarNode[] grammarNodes )
        {
            this.grammarNodes = new List<GrammarNode> ( grammarNodes );
        }

        /// <summary>
        /// Appends a node to this container's children list
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        public virtual T AppendNode ( GrammarNode grammarNode )
        {
            this.grammarNodes.Add ( grammarNode );
            return ( T ) this;
        }

        /// <summary>
        /// Appends a collection of nodes to this container's children list
        /// </summary>
        /// <param name="grammarNodes"></param>
        /// <returns></returns>
        public virtual T AppendNodes ( IEnumerable<GrammarNode> grammarNodes )
        {
            this.grammarNodes.AddRange ( grammarNodes );
            return ( T ) this;
        }
    }
}
