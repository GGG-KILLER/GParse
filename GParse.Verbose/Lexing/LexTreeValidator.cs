using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;
using GParse.Verbose.MathUtils;

namespace GParse.Verbose.Lexing
{
    internal class LexTreeValidator : IMatcherTreeVisitor
    {
        private readonly HashSet<String> Rules;
        private readonly Stack<BaseMatcher> ParentStack = new Stack<BaseMatcher> ( );
        private BaseMatcher Parent => this.ParentStack.Peek ( );

        public LexTreeValidator ( String[] rules )
        {
            this.Rules = new HashSet<String> ( rules );
        }

        public void Visit ( CharMatcher charMatcher )
        {
            // No validation needed.
        }

        public void Visit ( EOFMatcher eofMatcher )
        {
            // No validation needed.
        }

        public void Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            // No validation is possible.
        }

        public void Visit ( RangeMatcher RangeMatcher )
        {
            // No validation needed.
        }

        public void Visit ( CharListMatcher CharListMatcher )
        {
            // No validation needed.
        }

        public void Visit ( StringMatcher stringMatcher )
        {
            // No validation needed.
        }

        public void Visit ( SequentialMatcher SequentialMatcher )
        {
            this.ParentStack.Push ( SequentialMatcher );
            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
                matcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( AlternatedMatcher AlternatedMatcher )
        {
            this.ParentStack.Push ( AlternatedMatcher );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
                matcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( IgnoreMatcher ignoreMatcher )
        {
            this.ParentStack.Push ( ignoreMatcher );
            ignoreMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( JoinMatcher joinMatcher )
        {
            this.ParentStack.Push ( joinMatcher );
            joinMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( MarkerMatcher markerMatcher )
        {
            this.ParentStack.Push ( markerMatcher );
            markerMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( NegatedMatcher negatedMatcher )
        {
            this.ParentStack.Push ( negatedMatcher );
            negatedMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( OptionalMatcher optionalMatcher )
        {
            if ( this.Parent is OptionalMatcher )
                throw new InvalidLexerException ( "A repeated expression cannot have an optional expression as it's child. (infinite loop)" );
            this.ParentStack.Push ( optionalMatcher );
            optionalMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( RepeatedMatcher repeatedMatcher )
        {
            this.ParentStack.Push ( repeatedMatcher );
            repeatedMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( RulePlaceholder rulePlaceholder )
        {
            if ( !this.Rules.Contains ( rulePlaceholder.Name ) )
                throw new InvalidLexerException ( $"Undefined rule {rulePlaceholder.Name} used." );
        }

        public void Visit ( RuleWrapper ruleWrapper )
        {
            if ( !this.Rules.Contains ( ruleWrapper.Name ) )
                throw new InvalidLexerException ( $"Undefined rule {ruleWrapper.Name} used." );
            this.ParentStack.Push ( ruleWrapper );
            ruleWrapper.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( SavingMatcher savingMatcher )
        {
            // No validation logic yet.
            this.ParentStack.Push ( savingMatcher );
            savingMatcher.PatternMatcher.Accept ( this );
            this.ParentStack.Pop ( );
        }

        public void Visit ( LoadingMatcher loadingMatcher )
        {
            // No validation logic yet.
        }
    }
}
