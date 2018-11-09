using System;
using GParse.CLI.AST;
using GParse.Fluent;
using GParse.Fluent.Matchers;

namespace GParse.CLI
{
    public class CLIParser : FluentParser
    {
        private readonly String[] Expressions;
        private readonly Boolean Initialized;

        public CLIParser ( params String[] exprs )
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

                StringNode factory ( String _, Fluent.Parsing.MatchResult res ) =>
                    new StringNode ( res.Nodes as StringNode[], String.Join ( "", res.Strings ) );

                if ( name == "root" )
                    this.RootRule ( name, expression, factory );
                else
                    this.Rule ( name, expression, factory );
            }
        }
    }
}
