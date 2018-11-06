using System;
using System.Collections.Generic;
using GParse.CLI.AST;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Optimization;
using GParse.Fluent.Parsing;
using GParse.Fluent.Visitors;
using GUtils.CLI.Commands;
using GUtils.CLI.Commands.Errors;

namespace GParse.CLI
{
    internal class Program
    {
        private static Boolean ShouldRun = true;
        private static readonly ExpressionParser ExpressionParser = new ExpressionParser ( );
        private static readonly MatchTreeOptimizer TreeOptimizer = new MatchTreeOptimizer ( );
        private static readonly ExpressionReconstructor ExpressionReconstructor = new ExpressionReconstructor ( );
        private static readonly ValidExpressionGenerator ExpressionGenerator = new ValidExpressionGenerator ( repeatedMatcherLimit: 25 );

        public static void Main ( )
        {
            var cmdMan = new CommandManager ( );
            cmdMan.AddHelpCommand ( );
            cmdMan.LoadCommands<Program> ( null );

            while ( ShouldRun )
            {
                var line = "";
                try
                {
                    Console.Write ( ">> " );
                    cmdMan.Execute ( line = Console.ReadLine ( ) );
                }
                catch ( NonExistentCommandException ex )
                {
                    Console.WriteLine ( $"Command '{ex.Command}' does not exist. Use 'help' to list available commands." );
                }
                catch ( CommandInvocationException ex )
                {
                    Console.WriteLine ( $"An error happened when running '{ex.Command}': {ex.Message}" );
                }
                catch ( InputLineParseException ex )
                {
                    Console.WriteLine ( $"Invalid command: {ex.Message}" );
                    Console.WriteLine ( line );
                    Console.WriteLine ( new String ( ' ', ex.Offset ) + "^" );
                }
                catch ( Exception ex )
                {
                    Console.WriteLine ( $"An unexpected error happened when running the last command:\n{ex}" );
                    throw;
                }
            }
        }

        [Command ( "exit" )]
        public static void Exit ( )
            => ShouldRun = false;

        [HelpDescription ( "Creates a new CLIParser with the specified rules." )]
        [HelpExample ( @"c
number = \d{1, 10}
operation = number [+\-*/] number
root = operation" )]
        [Command ( "c" ), Command ( "create-parser" )]
        public static void CreateParser ( )
        {
            var rules = new List<String> ( );
            String line;
            while ( true )
            {
                Console.Write ( $"Rule #{rules.Count + 1:00}:" );
                if ( String.IsNullOrWhiteSpace ( line = Console.ReadLine ( ).Trim ( ) ) )
                    break;
                rules.Add ( line );
            }

            new CLICommandMode ( rules.ToArray ( ) )
                .Run ( );
        }

        [RawInput, Command ( "p" ), Command ( "parse" )]
        public static void Parse ( String expr )
        {
            try
            {
                var parser = new CLIParser ( $"root = {expr}" );
                Console.Write ( "expression> " );
                var node = parser.Parse ( Console.ReadLine ( ) ) as StringNode;
                Console.WriteLine ( node );
            }
            catch ( InvalidExpressionException ex )
            {
                Console.WriteLine ( $"Invalid expression: {ex.Location}: {ex.Message}" );
                Console.WriteLine ( $"{expr}" );
                Console.WriteLine ( new String ( ' ', ex.Location.Byte ) + "^" );
            }
        }

        [RawInput, Command ( "o" ), Command ( "optimize" ), Command ( "optimize-expr" )]
        public static void Optimize ( String expr )
            => Console.WriteLine ( ExpressionParser
                .Parse ( expr )
                .Accept ( TreeOptimizer )
                .Accept ( ExpressionReconstructor ) );

        [Command ( "or" ), Command ( "orepl" ), Command ( "optimize-expr-repl" )]
        public static void OptimizeRepl ( )
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

        [RawInput, Command ( "g" ), Command ( "gen" ), Command ( "generate-expr" ), Command ( "generate-valid-expr" )]
        public static void Generate ( String expr )
        {
            var i = 0;
            foreach ( var expression in ExpressionParser.Parse ( expr ).Accept ( ExpressionGenerator ) )
                Console.WriteLine ( $"gen-expr-{++i:0000}>>> [{expression.Length}]{expression}" );
        }

        [Command ( "gr" ), Command ( "grepl" ), Command ( "gen-repl" ), Command ( "generate-expr-repl" ), Command ( "generate-valid-expr-repl" )]
        public static void GenerateRepl ( )
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
