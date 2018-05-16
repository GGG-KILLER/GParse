using System;
using System.Collections.Generic;
using GParse.CLI.AST;
using GParse.Verbose;
using GParse.Verbose.Dbug;
using GParse.Verbose.Matchers;

namespace GParse.CLI
{
    internal class Program
    {
        private static readonly Stack<LoggerTimingArea> timings = new Stack<LoggerTimingArea> ( );

        private static void P_RuleExectionStarted ( String Name, VerboseParser.ParserState state )
        {
#pragma warning disable CC0022 // Should dispose object
            LoggerTimingArea area;
            timings.Push ( area = new LoggerTimingArea ( $"rule {Name}", timings.Peek ( ) ) );
#pragma warning restore CC0022 // Should dispose object
        }

        private static void P_RuleExecutionMatched ( String Name, String[] Content, VerboseParser.ParserState state )
        {
            timings.Peek ( ).Log ( $"[{Name}] Matched:              [{String.Join ( ", ", Content )}]" );
        }

        private static void P_RuleExectionEnded ( String Name, VerboseParser.ParserState prev, VerboseParser.ParserState curr )
        {
            timings.Pop ( ).Dispose ( );
        }

        internal class P : VerboseParser
        {
            public override void Setup ( )
            {
                BaseMatcher number = this.Rule ( "number", GetMatcher ( "[0-9]" ) * -1 + ~( GetMatcher ( "." ) + GetMatcher ( "[0-9]" ) * -1 ) );
                BaseMatcher ws     = this.Rule ( "ws", GetMatcher ( "[ \t]" ).Infinite ( ).Optional ( ), true );
                this.Rule ( "expr1", ws + number + ws + GetMatcher ( "[+-]" ) + ws + this.Rule ( "expr" ) + ws );
                this.Rule ( "expr", this.Rule ( "expr1" ) | number );
                this.SetRootRule ( "expr" );

                this.Factory ( "number", ( Name, ContentQueue, NodeQueue )
                    // Unused content from rules is discarded upon exit
                    => new NumberExpression ( Double.Parse ( String.Join ( "", ContentQueue ) ) ) );
                this.Factory ( "expr1", ( Name, ContentQueue, NodeQueue )
                    => new BinaryOperatorExpression ( ContentQueue.Dequeue ( ), NodeQueue.Dequeue ( ), NodeQueue.Dequeue ( ) ) );

                // These will just enqueue the item in the end of
                // the queue
                this.PassthroughFactory ( "expr" );

                this.Debug = true;
                Logger.WriteLine ( "Printing tree..." );
                this.PrintRules ( );
                this.Debug = false;
            }
        }

        private static void Main ( )
        {
            MatcherDebug.LogLine = Logger.WriteLine;
            Logger.WriteLine ( "Program started." );

            // Initialize the parser
            var p = new P ( );
            p.RuleExectionStarted += P_RuleExectionStarted;
            p.RuleExecutionMatched += P_RuleExecutionMatched;
            p.RuleExectionEnded += P_RuleExectionEnded;
            Logger.ShouldPrint = true;

            while ( true )
            {
                // Wait for keypress
                Console.ReadLine ( );

                // Clear
                Logger.ClearQueue ( );
                Console.Clear ( );

                // Then start logging again
                Logger.ShouldPrint = false;
                Logger.WriteLine ( new String ( '-', 70 ) );
                var line = Console.ReadLine ( );
                if ( line == "exit" )
                    break;

                // Attempt to parse
                try
                {
                    using ( var t = new LoggerTimingArea ( "Parsing" ) )
                    {
                        timings.Push ( t );
                        t.Log ( $"Result: {p.Parse ( line )}" );
                        timings.Pop ( );
                    }
                }
                catch ( Exception ex )
                {
                    Logger.WriteLine ( ex );
                }
                Logger.ShouldPrint = true;
            }
        }
    }
}
