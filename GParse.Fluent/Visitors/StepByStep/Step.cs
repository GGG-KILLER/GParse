using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Fluent.Visitors.StepByStep
{
    /// <summary>
    /// Represents a step in matching
    /// </summary>
    public readonly struct Step
    {
        /// <summary>
        /// The entire code snippet
        /// </summary>
        public readonly String Code;

        /// <summary>
        /// The error that occurred when matching
        /// </summary>
        public readonly ParsingException Error;

        /// <summary>
        /// The expression that was executing in this step
        /// </summary>
        public readonly String Expression;

        /// <summary>
        /// The starting location of the step
        /// </summary>
        public readonly SourceLocation Location;

        /// <summary>
        /// The resulting match of this step
        /// </summary>
        public readonly String[] Match;

        /// <summary>
        /// The source code range this step is from
        /// </summary>
        public readonly SourceRange Range;

        /// <summary>
        /// Whether this step was successful
        /// </summary>
        public readonly StepResult Result;

        /// <summary>
        /// Initializes this struct
        /// </summary>
        /// <param name="Expression"></param>
        /// <param name="Code"></param>
        /// <param name="Location"></param>
        /// <param name="Range"></param>
        public Step ( String Expression, String Code, SourceLocation Location, SourceRange Range )
        {
            this.Expression = Expression;
            this.Result = StepResult.NoResult;
            this.Code = Code;
            this.Location = Location;
            this.Range = Range;
            this.Match = null;
            this.Error = null;
        }

        /// <summary>
        /// Initializes this struct
        /// </summary>
        /// <param name="Expression"></param>
        /// <param name="Code"></param>
        /// <param name="Location"></param>
        /// <param name="Range"></param>
        /// <param name="Match"></param>
        public Step ( String Expression, String Code, SourceLocation Location, SourceRange Range, String[] Match )
        {
            this.Expression = Expression;
            this.Result = StepResult.Success;
            this.Code = Code;
            this.Location = Location;
            this.Range = Range;
            this.Match = Match;
            this.Error = null;
        }

        /// <summary>
        /// Initializes this struct
        /// </summary>
        /// <param name="Expression"></param>
        /// <param name="Code"></param>
        /// <param name="Location"></param>
        /// <param name="Range"></param>
        /// <param name="Error"></param>
        public Step ( String Expression, String Code, SourceLocation Location, SourceRange Range, ParsingException Error )
        {
            this.Expression = Expression;
            this.Result = StepResult.Failure;
            this.Code = Code;
            this.Location = Location;
            this.Range = Range;
            this.Match = null;
            this.Error = Error;
        }
    }
}
