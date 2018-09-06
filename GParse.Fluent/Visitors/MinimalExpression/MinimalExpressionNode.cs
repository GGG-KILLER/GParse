using System;
using System.Collections.Generic;

namespace GParse.Fluent.Visitors.MinimalExpression
{
    public struct MinimalExpressionNode
    {
        public List<MinimalExpressionNode> Children;
        public List<String> Values;

        public MinimalExpressionNode ( params String[] values )
        {
            this.Children = new List<MinimalExpressionNode> ( );
            this.Values = new List<String> ( values );
        }

        public MinimalExpressionNode ( params MinimalExpressionNode[] children )
        {
            this.Children = new List<MinimalExpressionNode> ( children );
            this.Values = new List<String> ( );
        }

        public static MinimalExpressionNode CreateNew ( )
        {
            return new MinimalExpressionNode
            {
                Children = new List<MinimalExpressionNode> ( ),
                Values = new List<String> ( )
            };
        }

        public void AddChild ( MinimalExpressionNode node ) => this.Children.Add ( node );

        public void AddChild ( String[] values )
        {
            var child = CreateNew ( );
            child.Values.AddRange ( values );
            this.Children.Add ( child );
        }

        public String[] Flatten ( )
        {
            if ( this.Children.Count < 1 && this.Values.Count < 1 )
                return Array.Empty<String> ( );

            var results = new HashSet<String> ( );
            if ( this.Children.Count > 0 )
            {
                foreach ( MinimalExpressionNode child in this.Children )
                {
                    if ( this.Values.Count > 0 )
                    {
                        foreach ( String subResult in child.Flatten ( ) )
                            foreach ( String value in this.Values )
                                results.Add ( value + subResult );
                    }
                    else
                        results.UnionWith ( child.Flatten ( ) );
                }
            }
            else
                results.UnionWith ( this.Values );

            var arr = new String[results.Count];
            results.CopyTo ( arr );
            return arr;
        }
    }
}
