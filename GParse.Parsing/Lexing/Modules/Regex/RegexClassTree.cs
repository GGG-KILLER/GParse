using System;
using System.Collections.Generic;
using GParse.Common.IO;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class RegexClassTree
    {
        private class TreeNode
        {
            public readonly Dictionary<Char, TreeNode> SubTree;
            public String Name;
            public Node Value;

            public TreeNode ( ) : this ( null, null )
            {
            }

            public TreeNode ( String className, Node value )
            {
                this.SubTree = new Dictionary<Char, TreeNode> ( );
                this.Name = className;
                this.Value = value;
            }
        }

        private readonly TreeNode Root = new TreeNode ( );

        public void AddClass ( String name, Node value )
        {
            TreeNode node = this.Root;
            for ( var i = 0; i < name.Length; i++ )
            {
                var ch = name[i];
                if ( !node.SubTree.ContainsKey ( ch ) )
                    node.SubTree[ch] = new TreeNode ( );

                node = node.SubTree[ch];
                if ( name.Length <= i + 1 )
                {
                    node.Name = name;
                    node.Value = value;
                }
            }
        }

        public (String, Node) FindClass ( SourceCodeReader reader )
        {
            var offset = -1;
            TreeNode node = this.Root;
            // Go as deep in the tree as we can
            while ( reader.ContentLeftSize > ++offset
                && node.SubTree.TryGetValue ( ( Char ) reader.Peek ( offset ), out TreeNode subTree ) )
                node = subTree;

            // Then return whatever the node we stopped at has (be
            // it null or an actual regex node)
            return (node.Name, node.Value);
        }

        public IEnumerable<(String, Node)> GetAll ( )
        {
            var queue = new Queue<TreeNode> ( );
            queue.Enqueue ( this.Root );

            while ( queue.Count > 0 )
            {
                TreeNode node = queue.Dequeue ( );
                foreach ( KeyValuePair<Char, TreeNode> kv in node.SubTree )
                    queue.Enqueue ( kv.Value );

                if ( node.Name != null && node.Value != null )
                    yield return (node.Name, node.Value);
            }
        }
    }
}
