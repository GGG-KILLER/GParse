using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors.StepByStep
{
    /// <summary>
    /// Executes a parser recording all steps taken
    /// </summary>
    public class StepByStepRecorder<NodeT> : IMatcherTreeVisitor<Step>
    {
        [ThreadStatic]
        private static readonly ExpressionReconstructor reconstructor = new ExpressionReconstructor ( );

        private readonly FluentParser<NodeT> Parser;
        private String Code;
        private SourceCodeReader Reader;
        private List<Step> Steps;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="parser"></param>
        public StepByStepRecorder ( FluentParser<NodeT> parser )
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Records the execution of the parser on the provided <paramref name="code"/>
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Step[] Execute ( String code )
        {
            try
            {
                this.Code = code;
                this.Reader = new SourceCodeReader ( code );
                this.Steps = new List<Step>
                {
                    this.Parser.RawRule ( this.Parser.RootName ).Accept ( this )
                };
                return this.Steps.ToArray ( );
            }
            finally
            {
                this.Code = null;
                this.Reader = null;
                this.Steps = null;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        public Step Visit ( SequentialMatcher SequentialMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = SequentialMatcher.Accept ( reconstructor );
            var matches = new List<String> ( );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public Step Visit ( AlternatedMatcher AlternatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = AlternatedMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expression, this.Code, start, start.To ( start ) ) );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
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
                new ParsingException ( start, $"Failed to match any of the patterns." ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public Step Visit ( CharMatcher charMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = charMatcher.Accept ( reconstructor );

            if ( this.Reader.HasContent && this.Reader.IsNext ( charMatcher.Filter ) )
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
                new ParsingException ( start, $"Expected '{charMatcher.Filter}' but got '{( Char ) this.Reader.Peek ( )}'" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        /// <returns></returns>
        public Step Visit ( RangeMatcher RangeMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = RangeMatcher.Accept ( reconstructor );
            if ( this.Reader.HasContent )
            {
                var peek = ( Char ) this.Reader.Peek ( );
                if ( RangeMatcher.Range.ValueIn ( peek ) )
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
                new ParsingException ( start, $"Failed to match character inside range [{RangeMatcher.Range.Start}, {RangeMatcher.Range.End}]" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public Step Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = $"f:{filterFuncMatcher.FullFilterName}";
            Char peek;

            if ( this.Reader.HasContent && filterFuncMatcher.Filter ( peek = ( Char ) this.Reader.Peek ( ) ) )
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
                new ParsingException ( start, "Failed to match character that satisfies the filter function." ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="CharListMatcher"></param>
        /// <returns></returns>
        public Step Visit ( CharListMatcher CharListMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = CharListMatcher.Accept ( reconstructor );
            Char peek;

            if ( this.Reader.HasContent && Array.IndexOf ( CharListMatcher.Whitelist, peek = ( Char ) this.Reader.Peek ( ) ) != -1 )
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
                new ParsingException ( start, $"Failed to match any char in the whitelist: [{String.Join ( ", ", CharListMatcher.Whitelist )}]" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public Step Visit ( StringMatcher stringMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expression = stringMatcher.Accept ( reconstructor );
            if ( this.Reader.HasContent && this.Reader.IsNext ( stringMatcher.StringFilter ) )
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
                new ParsingException ( start, $"Failed to match string '{stringMatcher.StringFilter}'" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
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
                    new ParsingException ( start, "Matched pattern when not matching was expected" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public Step Visit ( OptionalMatcher optionalMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Step step;
            var expr = optionalMatcher.Accept ( reconstructor );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            this.Steps.Add ( step = optionalMatcher.PatternMatcher.Accept ( this ) );
            return new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), step.Match ?? Array.Empty<String> ( ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
        public Step Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Int32 i;
            var expr = repeatedMatcher.Accept ( reconstructor );
            var matchlist = new List<String> ( );

            this.Steps.Add ( new Step ( expr, this.Code, start, start.To ( start ) ) );
            for ( i = 0; i < repeatedMatcher.Range.End; i++ )
            {
                Step subStep = repeatedMatcher.PatternMatcher.Accept ( this );
                this.Steps.Add ( subStep );
                if ( subStep.Result == StepResult.Failure )
                    break;
                matchlist.AddRange ( subStep.Match );
            }

            if ( repeatedMatcher.Range.Start > i )
            {
                SourceLocation end = this.Reader.Location;
                this.Reader.Rewind ( start );
                return new Step ( expr, this.Code, start, start.To ( end ), new ParsingException ( start, "Failed to match the pattern the minimum amount of times required" ) );
            }

            return new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), matchlist.ToArray ( ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public Step Visit ( EOFMatcher eofMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var expr = eofMatcher.Accept ( reconstructor );
            return this.Reader.IsAtEOF
                ? new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), Array.Empty<String> ( ) )
                : new Step ( expr, this.Code, start, start.To ( this.Reader.Location ), new ParsingException ( start, "Failed to match EOF." ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public Step Visit ( SavingMatcher savingMatcher )
            => throw new NotImplementedException ( );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public Step Visit ( LoadingMatcher loadingMatcher )
            => throw new NotImplementedException ( );
    }
}
