using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors.StepByStep
{
    public class StepByStepRecorder : IMatcherTreeVisitor<Step>
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
                this.Steps.Add ( this.Parser.RawRule ( this.Parser.RootName ).Accept ( this ) );
                return this.Steps.ToArray ( );
            }
            finally
            {
                this.Code = null;
                this.Reader = null;
                this.Steps = null;
            }
        }

        public Step Visit ( AllMatcher allMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = allMatcher.Accept ( reconstructor );
            var matches = new List<String> ( );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in allMatcher.PatternMatchers )
            {
                Step subStep = matcher.Accept ( this );
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

        public Step Visit ( AnyMatcher anyMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = anyMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in anyMatcher.PatternMatchers )
            {
                Step subStep = matcher.Accept ( this );
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

        public Step Visit ( CharMatcher charMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = charMatcher.Accept ( reconstructor );

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

        public Step Visit ( CharRangeMatcher charRangeMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = charRangeMatcher.Accept ( reconstructor );
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

        public Step Visit ( FilterFuncMatcher filterFuncMatcher )
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

        public Step Visit ( MultiCharMatcher multiCharMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = multiCharMatcher.Accept ( reconstructor );
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

        public Step Visit ( RulePlaceholder rulePlaceholder )
        {
            SourceLocation start = this.Reader.Location;
            var expr = rulePlaceholder.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            Step res = this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );
            return res.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), res.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), res.Match );
        }

        public Step Visit ( StringMatcher stringMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = stringMatcher.Accept ( reconstructor );
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

        public Step Visit ( IgnoreMatcher ignoreMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = ignoreMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = ignoreMatcher.PatternMatcher.Accept ( this ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr,
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

        public Step Visit ( JoinMatcher joinMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = joinMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = joinMatcher.PatternMatcher.Accept ( this ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), new[] { String.Join ( "", step.Match ) } );
        }

        public Step Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = negatedMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = negatedMatcher.PatternMatcher.Accept ( this ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), Array.Empty<String> ( ) )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ),
                    new ParseException ( start, "Matched pattern when not matching was expected" ) );
        }

        public Step Visit ( OptionalMatcher optionalMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = optionalMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = optionalMatcher.PatternMatcher.Accept ( this ) );
            return new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match ?? Array.Empty<String> ( ) );
        }

        public Step Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Int32 i;
            var expr = repeatedMatcher.Accept ( reconstructor );
            var matchlist = new List<String> ( );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            for ( i = 0; i < repeatedMatcher.Maximum; i++ )
            {
                Step subStep = repeatedMatcher.PatternMatcher.Accept ( this );
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

        public Step Visit ( RuleWrapper ruleWrapper )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( ruleWrapper );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = ruleWrapper.PatternMatcher.Accept ( this ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match );
        }

        public Step Visit ( MarkerMatcher markerMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = reconstructor.Visit ( markerMatcher );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = markerMatcher.PatternMatcher.Accept ( this ) );
            return step.Result == StepResult.Failure
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Error )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match );
        }

        public Step Visit ( EOFMatcher eofMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expr = eofMatcher.Accept ( reconstructor );
            return this.Reader.EOF ( )
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), Array.Empty<String> ( ) )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), new ParseException ( start, "Failed to match EOF." ) );
        }
    }
}
