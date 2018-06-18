using System;
using System.Collections.Generic;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public class EBNFReconstructor : MatcherTreeVisitor<String>
    {
        public override String Visit ( AllMatcher allMatcher )
        {
            return $"({String.Join ( ", ", Array.ConvertAll ( allMatcher.PatternMatchers, this.Visit ) )})";
        }

        public override String Visit ( AnyMatcher anyMatcher )
        {
            return $"({String.Join ( " | ", Array.ConvertAll ( anyMatcher.PatternMatchers, this.Visit ) )})";
        }

        public override String Visit ( CharMatcher charMatcher )
        {
            return $"'{charMatcher.Filter}'";
        }

        public override String Visit ( CharRangeMatcher charRangeMatcher )
        {
            var start = charRangeMatcher.Strict ? charRangeMatcher.Start : charRangeMatcher.Start - 1;
            var end = charRangeMatcher.Strict ? charRangeMatcher.End : charRangeMatcher.End + 1;
            return $"? interval (0x{start:X2}, 0x{end:X2}) ?";
        }

        public override String Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            return $"? {filterFuncMatcher.FullFilterName} ?";
        }

        public override String Visit ( MultiCharMatcher multiCharMatcher )
        {
            return $"( '{String.Join ( "' | '", multiCharMatcher.Whitelist )}' )";
        }

        public override String Visit ( RulePlaceholder rulePlaceholder )
        {
            return rulePlaceholder.Name;
        }

        public override String Visit ( StringMatcher stringMatcher )
        {
            return $"'{stringMatcher.StringFilter}'";
        }

        public override String Visit ( IgnoreMatcher ignoreMatcher )
        {
            return this.Visit ( ignoreMatcher.PatternMatcher );
        }

        public override String Visit ( JoinMatcher joinMatcher )
        {
            return this.Visit ( joinMatcher.PatternMatcher );
        }

        public override String Visit ( NegatedMatcher negatedMatcher )
        {
            return $"-{this.Visit ( negatedMatcher.PatternMatcher )}";
        }

        public override String Visit ( OptionalMatcher optionalMatcher )
        {
            return $"[{this.Visit ( optionalMatcher.PatternMatcher )}]";
        }

        public override String Visit ( RepeatedMatcher repeatedMatcher )
        {
            var list = new List<String> ( );
            for ( var i = 0; i < repeatedMatcher.Minimum; i++ )
                list.Add ( this.Visit ( repeatedMatcher.PatternMatcher ) );
            list.Add ( $"{this.Visit ( repeatedMatcher.PatternMatcher )} * {repeatedMatcher.Maximum}" );
            return $"( { String.Join ( ", ", list ) } )";
        }

        public override String Visit ( RuleWrapper ruleWrapper )
        {
            return $"{ruleWrapper.Name} = {this.Visit ( ruleWrapper.PatternMatcher )};";
        }

        public override String Visit ( MarkerMatcher markerMatcher )
        {
            return this.Visit ( markerMatcher.PatternMatcher );
        }

        public override String Visit ( EOFMatcher eofMatcher )
        {
            return "EOF";
        }
    }
}
