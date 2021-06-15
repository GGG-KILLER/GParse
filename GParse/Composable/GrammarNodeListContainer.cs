using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a node that contains other nodes as it's children
    /// </summary>
    /// <typeparam name="TNode">The node that is inheriting from this class.</typeparam>
    /// <typeparam name="TElem">
    /// The T of all the <see cref="GrammarNode{T}" /> contained by this node.
    /// </typeparam>
    public abstract class GrammarNodeListContainer<TNode, TElem> : GrammarNode<TElem>
        where TNode : GrammarNodeListContainer<TNode, TElem>
    {
        /// <summary>
        /// The list of grammar nodes
        /// </summary>
        public ImmutableArray<GrammarNode<TElem>> GrammarNodes { get; }

        /// <summary>
        /// Initializes a new <see cref="GrammarNodeListContainer{TNode,TElem}" />. Flattens nested
        /// nodes of the same type if requested.
        /// </summary>
        /// <param name="grammarNodes">The nodes to be added.</param>
        /// <param name="flatten">Whether to flatten the provided array.</param>
        /// <remarks>
        /// Flattening works as following: TNode[GrammarNode, GrammarNode, GrammarNode,
        /// TNode[GrammarNode, GrammarNode], GrammarNode] becomes TNode[GrammarNode, GrammarNode,
        /// GrammarNode, GrammarNode, GrammarNode, GrammarNode].
        /// </remarks>
        protected GrammarNodeListContainer(IEnumerable<GrammarNode<TElem>> grammarNodes, Boolean flatten)
        {
            if (grammarNodes is null)
                throw new ArgumentNullException(nameof(grammarNodes));

            ImmutableArray<GrammarNode<TElem>>.Builder builder = ImmutableArray.CreateBuilder<GrammarNode<TElem>>(grammarNodes.Count());
            foreach (GrammarNode<TElem> node in grammarNodes)
            {
                if (flatten && node is TNode container)
                {
                    builder.AddRange(container.GrammarNodes);
                }
                else
                {
                    builder.Add(node);
                }
            }
            this.GrammarNodes = builder.ToImmutable();
        }
    }
}