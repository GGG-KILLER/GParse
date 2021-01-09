using System;
using System.Collections.Generic;
using GParse.IO;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// This tree is only meant for use inside <see cref="ModularLexerBuilder{TTokenType}" /> and
    /// <see cref="ModularLexer{TTokenType}" />. If used anywhere else without knowing all implications it
    /// WILL GO BADLY.
    /// </summary>
    public sealed class LexerModuleTree<TTokenType>
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
            public readonly List<ILexerModule<TTokenType>> Values = new List<ILexerModule<TTokenType>> ( );

            /// <summary>
            /// The children of this node
            /// </summary>
            public readonly Dictionary<Char, TreeNode> Children = new Dictionary<Char, TreeNode> ( );

            /// <summary>
            /// Initializes a node
            /// </summary>
            /// <param name="parent"></param>
            public TreeNode ( TreeNode? parent )
            {
                this.Parent = parent;
            }
        }

        /// <summary>
        /// The root of the tree
        /// </summary>
        private readonly TreeNode _root = new TreeNode ( null );

        /// <summary>
        /// A module used as a fallback when all other modules fail to consume the remaining input.
        /// </summary>
        public ILexerModule<TTokenType>? FallbackModule { get; set; }

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="module"></param>
        public void AddChild ( ILexerModule<TTokenType> module )
        {
            if ( module is null )
                throw new ArgumentNullException ( nameof ( module ) );

            TreeNode node = this._root;
            if ( !String.IsNullOrEmpty ( module.Prefix ) )
            {
                for ( var i = 0; i < module.Prefix!.Length; i++ )
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
        /// Whether the value was removed (false means the module did not exist in the tree)
        /// </returns>
        public Boolean RemoveChild ( ILexerModule<TTokenType> module )
        {
            if ( module is null )
                throw new ArgumentNullException ( nameof ( module ) );

            TreeNode node = this._root;

            // Find the node based on prefix
            if ( !String.IsNullOrEmpty ( module.Prefix ) )
            {
                for ( var i = 0; i < module.Prefix!.Length; i++ )
                {
                    if ( !node.Children.TryGetValue ( module.Prefix[i], out TreeNode? tmpNode ) )
                        return false;
                    node = tmpNode;
                }
            }

            // Try to remove the value from the node
            if ( !node.Values.Remove ( module ) )
                return false;

            // Remove nodes with no children nor values
            while ( node.Parent != null && node.Values.Count == 0 && node.Children.Count == 0 )
            {
                TreeNode child = node;
                node = node.Parent;

                Char? k = null;
                foreach ( KeyValuePair<Char, TreeNode> kv in node.Children )
                {
                    if ( kv.Value == child )
                    {
                        k = kv.Key;
                        break;
                    }
                }
                if ( k.HasValue )
                    node.Children.Remove ( k.Value );
            }

            return true;
        }

        /// <summary>
        /// Return candidate modules based on whether their prefixes match what's at the start of what's
        /// left in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<ILexerModule<TTokenType>> GetSortedCandidates ( ICodeReader reader )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

            var candidates = new Stack<ILexerModule<TTokenType>> ( );
            var depth = 0;
            TreeNode? node = this._root;

            // This could probably be done in a better way.
            var charctersLeft = reader.Length - reader.Position;
            while ( true )
            {
                foreach ( ILexerModule<TTokenType> module in node.Values )
                    candidates.Push ( module );

                if ( charctersLeft <= depth
                     || reader.Peek ( depth ) is not Char peeked
                     || !node.Children.TryGetValue ( peeked, out node ) )
                {
                    break;
                }

                depth++;
            }

            return candidates;
        }
    }
}
