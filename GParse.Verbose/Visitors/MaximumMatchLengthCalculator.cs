using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors
{
    public class MaximumMatchLengthCalculator : IMatcherTreeVisitor<Int32>
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
                this.LengthCache[matcher] = matcher.Accept ( this );
            }
            return this.LengthCache[matcher];
        }

        public Int32 Visit ( AllMatcher allMatcher )
        {
            var maxes = Array.ConvertAll ( allMatcher.PatternMatchers, pm => pm.Accept ( this ) );
            var max = 0;
            for ( var i = 0; i < maxes.Length; i++ )
                max += maxes[i];
            return max;
        }

        public Int32 Visit ( AnyMatcher anyMatcher )
        {
            var maxes = Array.ConvertAll ( anyMatcher.PatternMatchers, pm => pm.Accept ( this ) );
            var max = -1;
            for ( var i = 0; i < maxes.Length; i++ )
                if ( maxes[i] > max )
                    max = maxes[i];
            return max;
        }

        public Int32 Visit ( CharMatcher charMatcher ) => 1;

        public Int32 Visit ( CharRangeMatcher charRangeMatcher ) => 1;

        public Int32 Visit ( FilterFuncMatcher filterFuncMatcher ) => 1;

        public Int32 Visit ( MultiCharMatcher multiCharMatcher ) => 1;

        public Int32 Visit ( RulePlaceholder rulePlaceholder )
        {
            return this.RuleStack.Contains ( rulePlaceholder.Name )
                ? 0
                : this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );
        }

        public Int32 Visit ( StringMatcher stringMatcher ) => stringMatcher.StringFilter.Length;

        public Int32 Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        public Int32 Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        public Int32 Visit ( NegatedMatcher negatedMatcher ) => negatedMatcher.PatternMatcher.Accept ( this );

        public Int32 Visit ( OptionalMatcher optionalMatcher ) => optionalMatcher.PatternMatcher.Accept ( this );

        // Clamp result of these to Int32.MaxValue since usually
        // Maximum is that
        public Int32 Visit ( RepeatedMatcher repeatedMatcher )
            => Math.Min ( repeatedMatcher.PatternMatcher.Accept ( this ) * repeatedMatcher.Maximum, Int32.MaxValue );

        public Int32 Visit ( RuleWrapper ruleWrapper ) => ruleWrapper.PatternMatcher.Accept ( this );

        public Int32 Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        public Int32 Visit ( EOFMatcher eofMatcher ) => 0;
    }
}
