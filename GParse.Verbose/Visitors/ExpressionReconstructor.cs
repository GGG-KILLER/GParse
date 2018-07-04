using System;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Utilities;

namespace GParse.Verbose.Visitors
{
    public class ExpressionReconstructor : IMatcherTreeVisitor<String>
    {
        private Boolean InRegexSet;

        public String Visit ( AllMatcher allMatcher )
            => $"( {String.Join ( " ", Array.ConvertAll ( allMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )} )";

        public String Visit ( AnyMatcher anyMatcher )
        {
            if ( Array.TrueForAll ( anyMatcher.PatternMatchers, matcher => matcher is CharMatcher || matcher is CharRangeMatcher
                || matcher is MultiCharMatcher ) )
            {
                this.InRegexSet = true;
                var repr = $"[{String.Join ( "", Array.ConvertAll ( anyMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )}]";
                this.InRegexSet = false;
                return repr;
            }
            else
                return $"( {String.Join ( " | ", Array.ConvertAll ( anyMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )} )";
        }

        public String Visit ( CharMatcher charMatcher )
        {
            var repr = StringUtilities.GetCharacterRepresentation ( charMatcher.Filter );
            return this.InRegexSet ? repr : $"'{repr}'";
        }

        public String Visit ( CharRangeMatcher charRangeMatcher )
        {
            var start = ( Char ) ( charRangeMatcher.Range.Start + 1 );
            var end = ( Char ) ( charRangeMatcher.Range.End - 1 );
            var startRepr = StringUtilities.GetCharacterRepresentation ( start );
            var endRepr = StringUtilities.GetCharacterRepresentation ( end );
            var repr = $"{startRepr}-{endRepr}";
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        public String Visit ( FilterFuncMatcher filterFuncMatcher )
            => throw new NotSupportedException ( "FilterFuncMatchers are not implemented in expressions." );

        public String Visit ( MultiCharMatcher multiCharMatcher )
        {
            var repr = String.Join ( "", Array.ConvertAll ( multiCharMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) );
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        public String Visit ( RulePlaceholder rulePlaceholder )
            => rulePlaceholder.Name;

        public String Visit ( StringMatcher stringMatcher )
            => $"'{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}'";

        public String Visit ( IgnoreMatcher ignoreMatcher )
        {
            return ignoreMatcher.PatternMatcher is MarkerMatcher markerMatcher
                ? $"im:({markerMatcher.PatternMatcher.Accept ( this )})"
                : $"i:({ignoreMatcher.PatternMatcher.Accept ( this )})";
        }

        public String Visit ( JoinMatcher joinMatcher )
            => $"j:({joinMatcher.PatternMatcher.Accept ( this )})";

        public String Visit ( NegatedMatcher negatedMatcher )
            => $"!({negatedMatcher.PatternMatcher.Accept ( this )})";

        public String Visit ( OptionalMatcher optionalMatcher )
            => $"({optionalMatcher.PatternMatcher.Accept ( this )})?";

        public String Visit ( RepeatedMatcher repeatedMatcher )
        {
            if ( repeatedMatcher.Range.Start == 0 && repeatedMatcher.Range.End == Int32.MaxValue )
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )})*";
            else if ( repeatedMatcher.Range.Start == 1 && repeatedMatcher.Range.End == Int32.MaxValue )
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )})+";
            else
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )}){{{repeatedMatcher.Range.Start}, {repeatedMatcher.Range.End}}}";
        }

        public String Visit ( RuleWrapper ruleWrapper )
            => ruleWrapper.PatternMatcher.Accept ( this );

        public String Visit ( MarkerMatcher markerMatcher )
            => $"m:({markerMatcher.PatternMatcher.Accept ( this )})";

        public String Visit ( EOFMatcher eofMatcher )
            => "EOF";
    }
}
