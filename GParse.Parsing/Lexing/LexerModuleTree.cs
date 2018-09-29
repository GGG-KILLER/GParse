using System;
using System.Collections.Generic;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing
{
    /// <summary>
    /// This tree is only meant for use inside
    /// <see cref="LexerBuilder" /> and <see cref="ModularLexer" />. If
    /// used anywhere else without knowing all implications it
    /// WILL GO BADLY.
    /// </summary>
    public class LexerModuleTree<TokenTypeT> where TokenTypeT : Enum
    {
        private class TreeNode
        {
            public readonly Char? Indexer;
            public readonly HashSet<ILexerModule<TokenTypeT>> Values = new HashSet<ILexerModule<TokenTypeT>> ( );
            public readonly Dictionary<Char?, TreeNode> Children = new Dictionary<Char?, TreeNode> ( );
        }

        private readonly TreeNode Root = new TreeNode ( );

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
                        node.Children[ch] = new TreeNode ( );
                    node = node.Children[ch];
                }
            }
            node.Values.Add ( module );
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

                var ch = reader.Peek ( depth );
                if ( node.Children.ContainsKey ( ch ) )
                {
                    node = node.Children[ch];
                    depth++;
                }
                else
                    break;
            }

            return candidates;
        }
    }
}
