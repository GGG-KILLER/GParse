using System;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Composable
{
    /// <summary>
    /// Represents an optional grammar node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Optional<T> : GrammarNodeContainer<T>
    {
        /// <summary>
        /// Initializes a new optional node.
        /// </summary>
        /// <param name="innerNode"><inheritdoc cref="GrammarNodeContainer{T}.InnerNode" path="/summary" /></param>
        public Optional ( GrammarNode<T> innerNode )
            : base ( innerNode is Optional<T> optional ? optional.InnerNode : innerNode )
        {
        }
    }

    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public partial class GrammarNodeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "node" )]
        public static Optional<T>? Optional<T> ( GrammarNode<T>? node ) =>
            node is null ? null : new Optional<T> ( node );
    }
}