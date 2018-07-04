using System;
using GParse.Verbose.Parsing;
using GParse.Verbose.Optimization;
using GParse.Verbose.Visitors;
using GUtils.CLI.Commands;

namespace GParse.CLI
{
    internal class Program
    {
        private static Boolean ShouldRun = true;
        private static readonly ExpressionParser ExpressionParser = new ExpressionParser ( );
        private static readonly MatchTreeOptimizer TreeOptimizer = new MatchTreeOptimizer ( );
        private static readonly ExpressionReconstructor ExpressionReconstructor = new ExpressionReconstructor ( );
        private static readonly ValidExpressionGenerator ExpressionGenerator = new ValidExpressionGenerator ( repeatedMatcherLimit: 25 );

        private static void Main ( )
        {
            var cmdMan = new CommandManager ( );
            cmdMan.LoadCommands ( typeof ( Program ) );

            while ( ShouldRun )
            {
                try
                {
                    Console.Write ( ">> " );
                    cmdMan.Execute ( Console.ReadLine ( ) );
                }
                catch ( Exception ex )
                {
                    Console.WriteLine ( ex.Message );
                }
            }
        }

        [Command ( "exit" )]
        private static void Exit ( )
            => ShouldRun = false;

        [Command ( "o" ), Command ( "optimize" ), Command ( "optimize-expr" )]
        private static void Optimize ( [CommandArgumentRest] String expr )
            => Console.WriteLine ( ExpressionParser.Parse ( expr ).Accept ( TreeOptimizer ).Accept ( ExpressionReconstructor ) );

        [Command ( "or" ), Command ( "orepl" ), Command ( "optimize-expr-repl" )]
        private static void OptimizeRepl ( )
        {
            Console.WriteLine ( "Tree Optimizer REPL. Input an empty line to exit." );
            do
            {
                Console.Write ( "optimize>>> " );
                var line = Console.ReadLine ( );
                if ( line == "" )
                    break;
                Optimize ( line );
            }
            while ( true );
        }

        [Command ( "g" ), Command ( "gen" ), Command ( "generate-expr" ), Command ( "generate-valid-expr" )]
        private static void Generate ( [CommandArgumentRest] String expr )
        {
            var i = 0;
            foreach ( var expression in ExpressionParser.Parse ( expr ).Accept ( ExpressionGenerator ) )
                Console.WriteLine ( $"gen-expr-{++i:0000}>>> [{expression.Length}]{expression}" );
        }

        [Command ( "gr" ), Command ( "grepl" ), Command ( "gen-repl" ), Command ( "generate-expr-repl" ), Command ( "generate-valid-expr-repl" )]
        private static void GenerateRepl ( )
        {
            Console.WriteLine ( "Valid Expression Generator REPL mode. Input an empty line to exit." );
            do
            {
                Console.Write ( "generate>>> " );
                var line = Console.ReadLine ( );
                if ( line == "" )
                    break;
                Generate ( line );
            }
            while ( true );
        }
    }
}
