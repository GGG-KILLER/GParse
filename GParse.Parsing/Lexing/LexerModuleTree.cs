using System;
using System.Collections.Generic;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    /// <summary>
    /// This tree is only meant for use inside
    /// <see cref="ModularLexerBuilder{TokenTypeT}" /> and
    /// <see cref="ModularLexer{TokenTypeT}" />. If used anywhere else without
    /// knowing all implications it WILL GO BADLY.
    /// </summary>
    public class LexerModuleTree<TokenTypeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// A node in the tree
        /// </summary>
        protected class TreeNode
        {
            /// <summary>
            /// The parent of this node
            /// </summary>
            public readonly TreeNode Parent;

            /// <summary>
            /// Initializes a node
            /// </summary>
            /// <param name="parent"></param>
            public TreeNode ( TreeNode parent )
            {
                this.Parent = parent;
            }

            /// <summary>
            /// The modules in this node
            /// </summary>
            public readonly HashSet<ILexerModule<TokenTypeT>> Values = new HashSet<ILexerModule<TokenTypeT>> ( );

            /// <summary>
            /// The children of this node
            /// </summary>
            public readonly Dictionary<Char?, TreeNode> Children = new Dictionary<Char?, TreeNode> ( );
        }

        /// <summary>
        /// The root of the tree
        /// </summary>
        protected readonly TreeNode Root = new TreeNode ( null );

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="module"></param>
        public void AddChild ( ILexerModule<TokenTypeT> module )
        {
            TreeNode node = this.Root;
            if ( !String.IsNullOrEmpty ( module.Prefix ) )
            {
                for ( var i = 0; i < module.Prefix.Length; i++ )
                {
                    var ch = module.Prefix[i];
                    if ( !node.Children.ContainsKey ( ch ) )
                        node.Children[ch] = new TreeNode ( node );
                    node = node.Children[ch];
                }
            }
            node.Values.Add ( module );
        }

        /// <summary>
        /// Removes a child module from the tree
        /// </summary>
        /// <param name="module">The module to be removed</param>
        /// <returns>
        /// Whether the value was removed (false means the module
        /// did not exist in the tree)
        /// </returns>
        public Boolean RemoveChild ( ILexerModule<TokenTypeT> module )
        {
            TreeNode node = this.Root;
            // Find the node based on prefix
            if ( !String.IsNullOrEmpty ( module.Prefix ) )
            {
                for ( var i = 0; i < module.Prefix.Length; i++ )
                    if ( !node.Children.TryGetValue ( module.Prefix[i], out node ) )
                        return false;
            }

            // Try to remove the value from the node
            if ( !node.Values.Remove ( module ) )
                return false;

            // Remove nodes with no children nor values
            while ( node.Values.Count == 0 && node.Children.Count == 0 )
            {
                TreeNode child = node;
                node = node.Parent;

                Char? k = null;
                foreach ( KeyValuePair<Char?, TreeNode> kv in node.Children )
                {
                    if ( kv.Value == child )
                    {
                        k = kv.Key;
                        break;
                    }
                }
                node.Children.Remove ( k );
            }

            return true;
        }

        /// <summary>
        /// Return candidate modules based on whether their
        /// prefixes match what's at the start of what's left in
        /// the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<ILexerModule<TokenTypeT>> GetSortedCandidates ( SourceCodeReader reader )
        {
            var candidates = new Stack<ILexerModule<TokenTypeT>> ( );
            var depth = 0;
            TreeNode node = this.Root;

            // This could probably be done in a better way.
            while ( true )
            {
                foreach ( ILexerModule<TokenTypeT> module in node.Values )
                    candidates.Push ( module );

                if ( reader.ContentLeft > depth && node.Children.TryGetValue ( reader.Peek ( depth ), out node ) )
                    depth++;
                else
                    break;
            }

            return candidates;
        }
    }
}
