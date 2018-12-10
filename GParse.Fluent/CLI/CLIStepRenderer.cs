using System;
using GParse.Fluent.Visitors.StepByStep;

namespace GParse.Fluent.CLI
{
    /// <summary>
    /// The class that renders the output of the <see cref="StepByStepRecorder{NodeT}"/>
    /// </summary>
    public class CLIStepRenderer
    {
        private readonly Step[] Steps;
        private Int32 Index;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="Steps"></param>
        public CLIStepRenderer ( Step[] Steps )
        {
            this.Steps = Steps;
            this.Index = 0;
        }

        #region Rendering

        #region Temporary color writing

        private static void TempColoredStringWrite ( Char value, ConsoleColor bg, ConsoleColor fg )
        {
            ConsoleColor ibg = Console.BackgroundColor;
            ConsoleColor ifg = Console.ForegroundColor;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.Write ( value );
            Console.BackgroundColor = ibg;
            Console.ForegroundColor = ifg;
        }

        private static void TempColoredStringWrite ( String value, ConsoleColor bg, ConsoleColor fg )
        {
            ConsoleColor ibg = Console.BackgroundColor;
            ConsoleColor ifg = Console.ForegroundColor;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.Write ( value );
            Console.BackgroundColor = ibg;
            Console.ForegroundColor = ifg;
        }

        private static void TempColoredStringWriteLine ( Char value, ConsoleColor bg, ConsoleColor fg )
        {
            ConsoleColor ibg = Console.BackgroundColor;
            ConsoleColor ifg = Console.ForegroundColor;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.WriteLine ( value );
            Console.BackgroundColor = ibg;
            Console.ForegroundColor = ifg;
        }

        private static void TempColoredStringWriteLine ( String value, ConsoleColor bg, ConsoleColor fg )
        {
            ConsoleColor ibg = Console.BackgroundColor;
            ConsoleColor ifg = Console.ForegroundColor;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.WriteLine ( value );
            Console.BackgroundColor = ibg;
            Console.ForegroundColor = ifg;
        }

        #endregion Temporary color writing

        private static void RenderHighlightedCode ( Step step, ConsoleColor bg = ConsoleColor.White, ConsoleColor fg = ConsoleColor.Black )
        {
            for ( var i = 0; i < step.Code.Length; i++ )
            {
                if ( step.Range.Start.Byte != step.Range.End.Byte
                    ? ( step.Range.Start.Byte <= i && i < step.Range.End.Byte )
                    : step.Range.Start.Byte == i )
                    TempColoredStringWrite ( step.Code[i], bg, fg );
                else
                    Console.Write ( step.Code[i] );
            }
        }

        private static void RenderStatus ( Step step )
        {
            Console.WriteLine ( $"Expression: {step.Expression}" );
            Console.WriteLine ( $"Location:   {step.Location}" );
            Console.WriteLine ( $"Range:      {step.Range}" );
            switch ( step.Result )
            {
                case StepResult.Success:
                    Console.Write ( "Result:     " );
                    TempColoredStringWriteLine ( "Success", ConsoleColor.Black, ConsoleColor.Green );
                    Console.WriteLine ( $"Match:       [{String.Join ( ", ", step.Match )}]" );
                    break;

                case StepResult.Failure:
                    Console.Write ( "Result:     " );
                    TempColoredStringWriteLine ( "Failure", ConsoleColor.Black, ConsoleColor.Red );
                    Console.Write ( $"Error:     " );
                    TempColoredStringWriteLine ( step.Error.Message, ConsoleColor.Black, ConsoleColor.Red );
                    break;

                case StepResult.NoResult:
                    Console.Write ( "Result:     " );
                    TempColoredStringWriteLine ( "None", ConsoleColor.Black, ConsoleColor.DarkGray );
                    break;
            }
        }

        #endregion Rendering

        /// <summary>
        /// Advance a step
        /// </summary>
        public void Advance ( )
        {
            if ( this.Index < this.Steps.Length - 1 )
                this.Index++;
        }

        /// <summary>
        /// Rollback a step
        /// </summary>
        public void Rewind ( )
        {
            if ( this.Index > 0 )
                this.Index--;
        }

        /// <summary>
        /// Whether this is the last step in the sequence
        /// </summary>
        public Boolean IsLastStep => this.Index == this.Steps.Length - 1;

        /// <summary>
        /// Render the step in the console screen
        /// </summary>
        public void Render ( )
        {
            Step current = this.Steps[this.Index];
            Console.WriteLine ( $"Step {this.Index}/{this.Steps.Length}" );
            RenderHighlightedCode ( current );
            Console.WriteLine ( );
            RenderStatus ( current );
        }
    }
}
