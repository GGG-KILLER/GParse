using System;
using GParse.CLI.AST;
using GParse.Fluent;
using GParse.Fluent.Parsing;

namespace GParse.CLI
{
    internal class CLIParser : FluentParser<StringNode>
    {
        private readonly String[] Expressions;
        private readonly Boolean Initialized;

        private static StringNode MarkerNodeFactory ( String name, MatchResult<StringNode> res ) =>
            new StringNode ( res.Nodes, String.Join ( "", res.Strings ) );

        public CLIParser ( params String[] exprs ) : base ( MarkerNodeFactory )
        {
            this.Expressions = exprs;
            this.Initialized = true;
            this.Setup ( );
        }

        protected override void Setup ( )
        {
            if ( !this.Initialized )
            {
                this.RootRule ( "root", "EOF" );
                return;
            }

            for ( var i = 0; i < this.Expressions.Length; i++ )
            {
                var assignment  = this.Expressions[i];
                var equalsIndex = assignment.IndexOf ( '=' );
                var name        = assignment.Substring ( 0, equalsIndex ).Trim ( );
                var expression  = assignment.Substring ( equalsIndex + 1 ).Trim ( );

                StringNode factory ( String _, MatchResult<StringNode> res ) =>
                    new StringNode ( res.Nodes as StringNode[], String.Join ( "", res.Strings ) );

                if ( name == "root" )
                    this.RootRule ( name, expression, factory );
                else
                    this.Rule ( name, expression, factory );
            }
        }
    }
}
