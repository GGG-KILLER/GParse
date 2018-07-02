using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Verbose.MathUtils;
using GParse.Verbose.Parsing;
using GParse.Verbose.Parsing.Matchers;
using GParse.Verbose.Parsing.Optimization;
using GParse.Verbose.Parsing.Visitors;
using GUtils.CLI.Commands;

namespace GParse.CLI
{
    internal class Program
    {
        private static Boolean ShouldRun = true;
        private static readonly MatchExpressionParser ExpressionParser = new MatchExpressionParser ( );
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
            foreach ( String expression in ExpressionParser.Parse ( expr ).Accept ( ExpressionGenerator ) )
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

        private static UInt32[] CartesianProduct ( IEnumerable<UInt32> lhs, IEnumerable<UInt32> rhs )
        {
            var list = new List<UInt32> ( );
            foreach ( UInt32 left in lhs )
                foreach ( UInt32 right in rhs )
                    list.Add ( left * right );
            return list.ToArray ( );
        }

        private static Boolean CanBeReducedHeavy ( RepeatedMatcher outer )
        {
            var inner = outer.PatternMatcher as RepeatedMatcher;
            var arr = CartesianProduct ( outer.Range.ToArray ( ), inner.Range.ToArray ( ) );

            for ( var i = 0; i < arr.Length - 1; i += 2 )
                if ( arr[i + 1] - arr[i] != 1 )
                    return false;
            return true;
        }

        private static Boolean CanBereducedMath ( RepeatedMatcher outer )
        {
            var inner = outer.PatternMatcher as RepeatedMatcher;
            return ( ( outer.Range.Start * inner.Range.Start ) + ( outer.Range.End * inner.Range.End ) ) % 2 == 0;
        }

        [Command ( "div" )]
        private static void GCDTest ( UInt32 max = 4 )
        {
            for ( var a = 2U; a <= max; a++ )
            {
                for ( var b = a; b <= max; b++ )
                {
                    for ( var c = 2U; c <= max; c++ )
                    {
                        for ( var d = c; d <= max; d++ )
                        {
                            var matcher = new RepeatedMatcher ( new RepeatedMatcher ( new CharMatcher ( 'a' ), new Range ( c, d ) ), new Range ( a, b ) );
                            Console.WriteLine ( $"expr{{{a}, {b}}}{{{c}, {d}}} can be reduced? {( CanBeReducedHeavy ( matcher ) ? "yes" : " no" )} | {( CanBereducedMath ( matcher ) ? "yes" : " no" )} (bruteforce | math)" );
                        }
                    }
                }
            }
        }
    }
}
