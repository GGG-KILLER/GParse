using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors
{
    public class MaximumMatchLengthCalculator : MatcherTreeVisitor<Int32>
    {
        private readonly Dictionary<BaseMatcher, Int32> LengthCache = new Dictionary<BaseMatcher, Int32> ( );
        private readonly VerboseParser Parser;
        private readonly Stack<String> RuleStack = new Stack<String> ( );
        public MaximumMatchLengthCalculator ( VerboseParser parser )
        {
            this.Parser = parser;
        }

        public Int32 Calculate ( BaseMatcher matcher )
        {
            if ( !this.LengthCache.ContainsKey ( matcher ) )
            {
                this.RuleStack.Clear ( );
                this.LengthCache[matcher] = this.Visit ( matcher );
            }
            return this.LengthCache[matcher];
        }

        public override Int32 Visit ( AllMatcher allMatcher )
        {
            var maxes = Array.ConvertAll ( allMatcher.PatternMatchers, this.Visit );
            var max = 0;
            for ( var i = 0; i < maxes.Length; i++ )
                max += maxes[i];
            return max;
        }

        public override Int32 Visit ( AnyMatcher anyMatcher )
        {
            var maxes = Array.ConvertAll ( anyMatcher.PatternMatchers, this.Visit );
            var max = -1;
            for ( var i = 0; i < maxes.Length; i++ )
                if ( maxes[i] > max )
                    max = maxes[i];
            return max;
        }

        public override Int32 Visit ( CharMatcher charMatcher ) => 1;

        public override Int32 Visit ( CharRangeMatcher charRangeMatcher ) => 1;

        public override Int32 Visit ( FilterFuncMatcher filterFuncMatcher ) => 1;

        public override Int32 Visit ( MultiCharMatcher multiCharMatcher ) => 1;

        public override Int32 Visit ( RulePlaceholder rulePlaceholder )
        {
            return this.RuleStack.Contains ( rulePlaceholder.Name )
                ? 0
                : this.Visit ( this.Parser.RawRule ( rulePlaceholder.Name ) );
        }

        public override Int32 Visit ( StringMatcher stringMatcher ) => stringMatcher.StringFilter.Length;

        public override Int32 Visit ( IgnoreMatcher ignoreMatcher ) => this.Visit ( ignoreMatcher.PatternMatcher );

        public override Int32 Visit ( JoinMatcher joinMatcher ) => this.Visit ( joinMatcher.PatternMatcher );

        public override Int32 Visit ( NegatedMatcher negatedMatcher ) => this.Visit ( negatedMatcher.PatternMatcher );

        public override Int32 Visit ( OptionalMatcher optionalMatcher ) => this.Visit ( optionalMatcher.PatternMatcher );

        // Clamp result of these to Int32.MaxValue since usually Maximum is that
        public override Int32 Visit ( RepeatedMatcher repeatedMatcher )
            => Math.Min ( this.Visit ( repeatedMatcher.PatternMatcher ) * repeatedMatcher.Maximum, Int32.MaxValue );

        public override Int32 Visit ( RuleWrapper ruleWrapper ) => this.Visit ( ruleWrapper.PatternMatcher );

        public override Int32 Visit ( MarkerMatcher markerMatcher ) => this.Visit ( markerMatcher.PatternMatcher );

        public override Int32 Visit ( EOFMatcher eofMatcher ) => 0;
    }
}
