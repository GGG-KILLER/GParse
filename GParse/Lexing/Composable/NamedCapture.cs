using System;
using GParse.Composable;
using GParse.Lexing.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that stores the matched value under a name
    /// </summary>
    public class NamedCapture : GrammarNode<Char>
    {
        /// <summary>
        /// The name of the node to store the match under
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// The grammar node whose match will be stored under the <see cref="Name"/>
        /// </summary>
        public GrammarNode<Char> InnerNode { get; }

        /// <summary>
        /// Initializes this node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        public NamedCapture ( String name, GrammarNode<Char> node )
        {
            this.Name = name;
            this.InnerNode = node;
        }
    }

    /// <summary>
    /// The class that contains all extension methods for dealing with grammar nodes
    /// (The name of this class is huge to avoid conflicts with other classes)
    /// </summary>
    public static partial class GParseLexingComposableExtensionMethods
    {
        /// <summary>
        /// Stores the value matched by this node under a name on the matches group
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamedCapture AsNamed ( this GrammarNode<Char> node, String name ) =>
            new NamedCapture ( name, node );
    }
}