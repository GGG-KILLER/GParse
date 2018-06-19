using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.MatcherTreeVisitors.StepByStep
{
    public class StepByStepRecorder : MatcherTreeVisitor<Step>
    {
        [ThreadStatic]
        private static readonly ExpressionReconstructor reconstructor = new ExpressionReconstructor ( );

        private readonly VerboseParser Parser;
        private String Code;
        private SourceCodeReader Reader;
        private List<Step> Steps;

        public StepByStepRecorder ( VerboseParser parser )
        {
            this.Parser = parser;
        }

        public Step[] Execute ( String Code )
        {
            try
            {
                this.Code = Code;
                this.Reader = new SourceCodeReader ( Code );
                this.Steps = new List<Step> ( );
                this.Steps.Add ( this.Visit ( this.Parser.RawRule ( this.Parser.RootName ) ) );
                return this.Steps.ToArray ( );
            }
            finally
            {
                this.Code = null;
                this.Reader = null;
                this.Steps = null;
            }
        }

        public override Step Visit ( AllMatcher allMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( allMatcher );
            var matches = new List<String> ( );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in allMatcher.PatternMatchers )
            {
                Step subStep = this.Visit ( matcher );
                this.Steps.Add ( subStep );
                if ( subStep.Result == StepResult.Failure )
                {
                    SourceLocation end = this.Reader.Location;
                    this.Reader.Rewind ( start );
                    return new Step ( expression, this.Code, start, start.To ( end ), subStep.Error );
                }
                else if ( subStep.Result == StepResult.Success )
                    matches.AddRange ( subStep.Match );
            }

            return new Step ( expression, this.Code, start, start.To ( this.Reader.Location ), matches.ToArray ( ) );
        }

        public override Step Visit ( AnyMatcher anyMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( anyMatcher );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in anyMatcher.PatternMatchers )
            {
                Step subStep = this.Visit ( matcher );
                this.Steps.Add ( subStep );
                if ( subStep.Result == StepResult.Success )
                    return new Step ( expression, this.Code, start, start.To ( this.Reader.Location ), subStep.Match );
            }
            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, $"Failed to match any of the patterns." ) );
        }

        public override Step Visit ( CharMatcher charMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( charMatcher );

            if ( !this.Reader.EOF ( ) && this.Reader.IsNext ( charMatcher.Filter ) )
            {
                this.Reader.Advance ( 1 );
                return new Step ( expression,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    new[] { charMatcher.StringFilter } );
            }

            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, $"Expected '{charMatcher.Filter}' but got '{( Char ) this.Reader.Peek ( )}'" ) );
        }

        public override Step Visit ( CharRangeMatcher charRangeMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( charRangeMatcher );
            if ( !this.Reader.EOF ( ) )
            {
                var peek = ( Char ) this.Reader.Peek ( );
                if ( charRangeMatcher.Start < peek && peek < charRangeMatcher.End )
                {
                    this.Reader.Advance ( 1 );
                    return new Step ( expression,
                        this.Code,
                        start,
                        start.To ( this.Reader.Location ),
                        new[] { peek.ToString ( ) } );
                }
            }

            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, $"Failed to match character inside range ({charRangeMatcher.Start}, {charRangeMatcher.End})" ) );
        }

        public override Step Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = $"f:{filterFuncMatcher.FullFilterName}";
            Char peek;

            if ( !this.Reader.EOF ( ) && filterFuncMatcher.Filter ( peek = ( Char ) this.Reader.Peek ( ) ) )
            {
                this.Reader.Advance ( 1 );
                return new Step ( expression,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    new[] { peek.ToString ( ) } );
            }

            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, "Failed to match character that satisfies the filter function." ) );
        }

        public override Step Visit ( MultiCharMatcher multiCharMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( multiCharMatcher );
            Char peek;

            if ( !this.Reader.EOF ( ) && Array.IndexOf ( multiCharMatcher.Whitelist, peek = ( Char ) this.Reader.Peek ( ) ) != -1 )
            {
                this.Reader.Advance ( 1 );
                return new Step ( expression,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    new[] { peek.ToString ( ) } );
            }

            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, $"Failed to match any char in the whitelist: [{String.Join ( ", ", multiCharMatcher.Whitelist )}]" ) );
        }

        public override Step Visit ( RulePlaceholder rulePlaceholder )
        {
            SourceLocation start = this.Reader.Location;
            var expr = reconstructor.Visit ( rulePlaceholder );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            Step res = this.Visit ( this.Parser.RawRule ( rulePlaceholder.Name ) );
            return res.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), res.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), res.Match );
        }

        public override Step Visit ( StringMatcher stringMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = reconstructor.Visit ( stringMatcher );
            if ( !this.Reader.EOF ( ) && this.Reader.IsNext ( stringMatcher.StringFilter ) )
            {
                this.Reader.Advance ( stringMatcher.StringFilter.Length );
                return new Step ( expression,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    new[] { stringMatcher.StringFilter } );
            }
            return new Step ( expression,
                this.Code,
                start,
                start.To ( this.Reader.Location ),
                new ParseException ( start, $"Failed to match string '{stringMatcher.StringFilter}'" ) );
        }

        public override Step Visit ( IgnoreMatcher ignoreMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( ignoreMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( ignoreMatcher.PatternMatcher ) );
            return step.Result == StepResult.Failure
                ?  new Step ( expr,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    step.Error )
                : new Step ( expr,
                    this.Code,
                    start,
                    start.To ( this.Reader.Location ),
                    Array.Empty<String> ( ) );
        }

        public override Step Visit ( JoinMatcher joinMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( joinMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( joinMatcher.PatternMatcher ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), new[] { String.Join ( "", step.Match ) } );
        }

        public override Step Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( negatedMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( negatedMatcher.PatternMatcher ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), Array.Empty<String> ( ) )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ),
                    new ParseException ( start, "Matched pattern when not matching was expected" ) );
        }

        public override Step Visit ( OptionalMatcher optionalMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( optionalMatcher.PatternMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( optionalMatcher.PatternMatcher ) );
            return new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match ?? Array.Empty<String> ( ) );
        }

        public override Step Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Int32 i;
            var expr = reconstructor.Visit ( repeatedMatcher );
            var matchlist = new List<String> ( );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            for ( i = 0; i < repeatedMatcher.Maximum; i++ )
            {
                Step subStep = this.Visit ( repeatedMatcher.PatternMatcher );
                this.Steps.Add ( subStep );
                if ( subStep.Result == StepResult.Failure )
                    break;
                matchlist.AddRange ( subStep.Match );
            }

            if ( repeatedMatcher.Minimum > i )
            {
                SourceLocation end = this.Reader.Location;
                this.Reader.Rewind ( start );
                return new Step ( expr, this.Code, start, start.To ( end ), new ParseException ( start, "Failed to match the pattern the minimum amount of times required" ) );
            }

            return new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), matchlist.ToArray ( ) );
        }

        public override Step Visit ( RuleWrapper ruleWrapper )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( ruleWrapper );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( ruleWrapper.PatternMatcher ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match );
        }

        public override Step Visit ( MarkerMatcher markerMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( markerMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = this.Visit ( markerMatcher.PatternMatcher ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match );
        }

        public override Step Visit ( EOFMatcher eofMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expr = reconstructor.Visit ( eofMatcher );
            return this.Reader.EOF ( )
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), Array.Empty<String> ( ) )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), new ParseException ( start, "Failed to match EOF." ) );
        }
    }
}
