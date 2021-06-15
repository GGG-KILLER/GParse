using System;
using System.Collections.Generic;
using System.Linq;
using GParse.IO;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// The delegate tree for lexer delegates
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public sealed class LexerDelegateTree<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// A node in the tree
        /// </summary>
        private sealed class TreeNode
        {
            /// <summary>
            /// The parent of this node
            /// </summary>
            public readonly TreeNode? Parent;

            /// <summary>
            /// The modules in this node
            /// </summary>
            public readonly List<TokenLexerDelegate<TTokenType>> Values = new();

            /// <summary>
            /// The children of this node
            /// </summary>
            public readonly Dictionary<char, TreeNode> Children = new();

            /// <summary>
            /// Initializes a node
            /// </summary>
            /// <param name="parent"></param>
            public TreeNode(TreeNode? parent)
            {
                Parent = parent;
            }
        }

        /// <summary>
        /// The root of the tree
        /// </summary>
        private readonly TreeNode _root = new(null);

        /// <summary>
        /// A module used as a fallback when all other modules fail to consume the remaining input.
        /// </summary>
        public TokenLexerDelegate<TTokenType>? FallbackDelegate { get; set; }

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="parserDelegate"></param>
        public void AddChild(string? prefix, TokenLexerDelegate<TTokenType> parserDelegate)
        {
            if (parserDelegate is null)
                throw new ArgumentNullException(nameof(parserDelegate));

            var node = _root;
            if (!string.IsNullOrEmpty(prefix))
            {
                for (var i = 0; i < prefix!.Length; i++)
                {
                    var ch = prefix[i];
                    if (!node.Children.ContainsKey(ch))
                        node.Children[ch] = new TreeNode(node);
                    node = node.Children[ch];
                }
            }

            node.Values.Add(parserDelegate);
        }

        /// <summary>
        /// Removes a child module from the tree
        /// </summary>
        /// <param name="prefix">The prefix to be cleared</param>
        /// <returns>
        /// Whether the value was removed (false means the module did not exist in the tree)
        /// </returns>
        public bool ClearPrefix(string? prefix)
        {
            var node = _root;

            // Find the node based on prefix
            if (!string.IsNullOrEmpty(prefix))
            {
                for (var i = 0; i < prefix!.Length; i++)
                {
                    if (!node.Children.TryGetValue(prefix[i], out var tmpNode))
                        return false;
                    node = tmpNode;
                }
            }

            // Try to remove the value from the node
            if (!node.Values.Any())
                return false;
            node.Values.Clear();

            // Remove nodes with no children nor values
            while (node.Parent != null && node.Values.Count == 0 && node.Children.Count == 0)
            {
                var child = node;
                node = node.Parent;

                char? k = null;
                foreach (var kv in node.Children)
                {
                    if (kv.Value == child)
                    {
                        k = kv.Key;
                        break;
                    }
                }
                if (k.HasValue)
                    node.Children.Remove(k.Value);
            }

            return true;
        }

        /// <summary>
        /// Return candidate modules based on whether their prefixes match what's at the start of what's
        /// left in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<TokenLexerDelegate<TTokenType>> GetSortedCandidates(ICodeReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var candidates = new Stack<TokenLexerDelegate<TTokenType>>();
            var depth = 0;
            var node = _root;

            // This could probably be done in a better way.
            var charctersLeft = reader.Length - reader.Position;
            while (true)
            {
                foreach (var value in node.Values)
                    candidates.Push(value);

                if (charctersLeft <= depth
                     || reader.Peek(depth) is not char peeked
                     || !node.Children.TryGetValue(peeked, out node))
                {
                    break;
                }

                depth++;
            }

            return candidates;
        }
    }
}
