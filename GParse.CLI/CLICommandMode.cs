using System;
using System.Collections.Generic;
using System.Reflection;
using GParse.CLI.AST;
using GParse.Fluent;
using GParse.Fluent.Matchers;
using GParse.Fluent.Visitors;
using GParse.Fluent.Visitors.MinimalExpression;
using GParse.Fluent.Visitors.StepByStep;
using GUtils.CLI.Commands;
using GUtils.CLI.Commands.Errors;

namespace GParse.CLI
{
    public class CLICommandMode
    {
        private Boolean Running;
        private readonly CLIParser Parser;
        private readonly IReadOnlyDictionary<String, BaseMatcher> Rules;
        private readonly ExpressionReconstructor ExpressionReconstructor;
        private readonly MaximumMatchLengthCalculator<StringNode> MaximumMatchLengthCalculator;
        private readonly EBNFReconstructor EBNFReconstructor;
        private readonly StepByStepRecorder<StringNode> StepByStepRecorder;
        private readonly MinimalExpressionsGenerator<StringNode> MinimalExpressionsGenerator;

        public CLICommandMode ( String[] expressions )
        {
            this.Parser = new CLIParser ( expressions );
            FieldInfo dict = typeof ( FluentParser<StringNode> ).GetField ( "Rules", BindingFlags.NonPublic | BindingFlags.Instance );
            this.Rules = dict.GetValue ( this.Parser ) as Dictionary<String, BaseMatcher>;

            this.ExpressionReconstructor = new ExpressionReconstructor ( );
            this.EBNFReconstructor = new EBNFReconstructor ( );

            this.MaximumMatchLengthCalculator = new MaximumMatchLengthCalculator<StringNode> ( this.Parser );
            this.StepByStepRecorder = new StepByStepRecorder<StringNode> ( this.Parser );
            this.MinimalExpressionsGenerator = new MinimalExpressionsGenerator<StringNode> ( this.Parser );
        }

        [Command ( "p" ), Command ( "exprs" )]
        public void PrintExpressions ( )
        {
            foreach ( KeyValuePair<String, BaseMatcher> kv in this.Rules )
                Console.WriteLine ( $"{kv.Key} = {kv.Value.Accept ( this.ExpressionReconstructor )}" );
        }

        [Command ( "ebnf" )]
        public void PrintEBNF ( )
        {
            foreach ( KeyValuePair<String, BaseMatcher> kv in this.Rules )
                Console.WriteLine ( $"{kv.Key} := {kv.Value.Accept ( this.EBNFReconstructor )}" );
        }

        [Command ( "parse" ), RawInput]
        public void Parse ( String value ) => Console.WriteLine ( this.Parser.Parse ( value ) );

        [Command ( "m" ), Command ( "min" )]
        public void GenerateMinimal ( )
        {
            var expressions = this.MinimalExpressionsGenerator.Generate ( this.Rules["root"] );
            foreach ( var expression in expressions )
                Console.WriteLine ( expression );
        }

        [Command ( "gen" ), Command ( "generate" )]
        public void Generate ( Boolean yolo = false, UInt32 maxReps = 5 )
        {
            var gen = new ValidExpressionGenerator<StringNode> ( this.Parser, yolo, maxReps );
            foreach ( String value in this.Rules["root"].Accept ( gen ) )
                Console.WriteLine ( new String ( '-', 40 ) + Environment.NewLine + value );
        }

        [Command ( "exit" )]
        public void Exit ( ) => this.Running = false;

        public void Run ( )
        {
            this.Running = true;
            var cmdMan = new CommandManager ( );
            cmdMan.LoadCommands ( typeof ( CLICommandMode ), this );
            cmdMan.AddHelpCommand ( );

            while ( this.Running )
            {
                var line = "";
                try
                {
                    Console.Write ( "parser mode>> " );
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
                    Console.WriteLine ( $"An unexpected error happened when running the last command: {line}\n{ex}" );
                }
            }
        }
    }
}
