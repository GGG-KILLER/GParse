using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that stores the matched value under a name
    /// </summary>
    public sealed class NamedCapture : GrammarNodeContainer<Char>
    {
        /// <summary>
        /// The name of the node to store the match under
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Initializes this node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        public NamedCapture ( String name, GrammarNode<Char> node ) : base ( node )
        {
            this.Name = name ?? throw new ArgumentNullException ( nameof ( name ) );
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?<{this.Name}>{GrammarNodeToStringConverter.Convert ( this.InnerNode )})";
    }

    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public static partial class GrammarNodeExtensions
    {
        /// <summary>
        /// Stores the value matched by this node under a name on the matches group
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "node" )]
        public static NamedCapture? AsNamed ( this GrammarNode<Char>? node, String name ) =>
            node is null ? null : new NamedCapture ( name, node );
    }
}