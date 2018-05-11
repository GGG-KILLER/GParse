using System;
using System.Collections.Generic;
using GParse.CLI.AST;
using GParse.Verbose;
using GParse.Verbose.Matchers;
using GUtils.Timing;

namespace GParse.CLI
{
    internal class P : VerboseParser
    {
        public override void Setup ( )
        {
            this.Debug = true;
            BaseMatcher number = this.Rule ( "number", GetMatcher ( "[0-9]" ) * -1 + ~( GetMatcher ( "." ) + GetMatcher ( "[0-9]" ) * -1 ) );
            BaseMatcher ws     = this.Rule ( "ws", GetMatcher ( "[ \t]" ).Infinite ( ).Optional ( ), true );
            this.Rule ( "expr", ws + number + ws + GetMatcher ( "[+-]" ) + ws + number + ws );
            this.SetRootRule ( "expr" );

            this.Factory ( "number", ( Name, ContentStack, NodeStack ) =>
            {
                return new NumberExpression ( Double.Parse ( ContentStack.Pop ( ) ) );
            } );
            this.Factory ( "expr", ( Name, ContentStack, NodeStack ) =>
            {
                return new BinaryOperatorExpression ( ContentStack.Pop ( ), NodeStack.Pop ( ), NodeStack.Pop ( ) );
            } );

            this.PrintTree ( );
        }
    }

    internal class Program
    {
        private static readonly Stack<TimingArea> timings = new Stack<TimingArea> ( );

        private static void Main ( )
        {
            var p = new P ( );
            Console.ReadKey ( true );
            p.RuleExectionStarted += P_RuleExectionStarted;
            p.RuleExecutionMatched += P_RuleExecutionMatched;
            p.RuleExectionEnded += P_RuleExectionEnded;
            while ( true )
            {
                Console.Clear ( );
                var line = Console.ReadLine ( );
                if ( line == "exit" )
                    break;
                Console.WriteLine ( p.Parse ( line ) );
            }
        }

        private static void P_RuleExectionStarted ( String Name )
        {
#pragma warning disable CC0022 // Should dispose object
            timings.Push ( new TimingArea ( $"rule {Name}", timings.Count > 0 ? timings.Peek ( ) : null ) );
#pragma warning restore CC0022 // Should dispose object
        }

        private static void P_RuleExecutionMatched ( String Name, String Content )
        {
            timings.Peek ( ).Log ( $"Matched: {Content}" );
        }

        private static void P_RuleExectionEnded ( String obj )
        {
            timings.Pop ( ).Dispose ( );
        }
    }
}
