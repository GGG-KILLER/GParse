using System.Collections;
using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a node that contains other nodes as it's children
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TElem"></typeparam>
    public abstract class GrammarNodeListContainer<TNode, TElem> : GrammarNode<TElem>, IEnumerable<GrammarNode<TElem>>
        where TNode : GrammarNodeListContainer<TNode, TElem>
    {
        /// <summary>
        /// The list of grammar nodes
        /// </summary>
        protected readonly List<GrammarNode<TElem>> grammarNodes;

        /// <summary>
        /// Initializes a new <see cref="GrammarNodeListContainer{TNode,TElem}" />
        /// </summary>
        /// <param name="grammarNodes"></param>
        protected GrammarNodeListContainer ( GrammarNode<TElem>[] grammarNodes )
        {
            this.grammarNodes = new List<GrammarNode<TElem>> ( grammarNodes );
        }

        /// <inheritdoc />
        public IEnumerator<GrammarNode<TElem>> GetEnumerator ( ) =>
            this.grammarNodes.GetEnumerator ( );

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator ( ) =>
            this.grammarNodes.GetEnumerator ( );
    }
}