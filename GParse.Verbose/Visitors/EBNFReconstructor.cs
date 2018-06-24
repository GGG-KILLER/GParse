using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors
{
    public class EBNFReconstructor : IMatcherTreeVisitor<String>
    {
        public String Visit ( AllMatcher allMatcher )
        {
            return $"({String.Join ( ", ", Array.ConvertAll ( allMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";
        }

        public String Visit ( AnyMatcher anyMatcher )
        {
            return $"({String.Join ( " | ", Array.ConvertAll ( anyMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";
        }

        public String Visit ( CharMatcher charMatcher )
        {
            return $"'{charMatcher.Filter}'";
        }

        public String Visit ( CharRangeMatcher charRangeMatcher )
        {
            var start = charRangeMatcher.Start - 1;
            var end = charRangeMatcher.End + 1;
            return $"? interval (0x{start:X2}, 0x{end:X2}) ?";
        }

        public String Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            return $"? {filterFuncMatcher.FullFilterName} ?";
        }

        public String Visit ( MultiCharMatcher multiCharMatcher )
        {
            return $"( '{String.Join ( "' | '", multiCharMatcher.Whitelist )}' )";
        }

        public String Visit ( RulePlaceholder rulePlaceholder )
        {
            return rulePlaceholder.Name;
        }

        public String Visit ( StringMatcher stringMatcher )
        {
            return $"'{stringMatcher.StringFilter}'";
        }

        public String Visit ( IgnoreMatcher ignoreMatcher )
        {
            return ignoreMatcher.PatternMatcher.Accept ( this );
        }

        public String Visit ( JoinMatcher joinMatcher )
        {
            return joinMatcher.PatternMatcher.Accept ( this );
        }

        public String Visit ( NegatedMatcher negatedMatcher )
        {
            return $"-{negatedMatcher.PatternMatcher.Accept ( this )}";
        }

        public String Visit ( OptionalMatcher optionalMatcher )
        {
            return $"[{optionalMatcher.PatternMatcher.Accept ( this )}]";
        }

        public String Visit ( RepeatedMatcher repeatedMatcher )
        {
            var list = new List<String> ( );
            for ( var i = 0; i < repeatedMatcher.Minimum; i++ )
                list.Add ( repeatedMatcher.PatternMatcher.Accept ( this ) );
            list.Add ( $"{repeatedMatcher.PatternMatcher.Accept ( this )} * {repeatedMatcher.Maximum}" );
            return $"( { String.Join ( ", ", list ) } )";
        }

        public String Visit ( RuleWrapper ruleWrapper )
        {
            return $"{ruleWrapper.Name} = {ruleWrapper.PatternMatcher.Accept ( this )};";
        }

        public String Visit ( MarkerMatcher markerMatcher )
        {
            return markerMatcher.PatternMatcher.Accept ( this );
        }

        public String Visit ( EOFMatcher eofMatcher )
        {
            return "EOF";
        }
    }
}
