using System;
using GParse.Verbose.Matchers;
using GParse.Verbose.Utilities;

namespace GParse.Verbose.Dbug
{
    public class ExpressionReconstructor : MatcherTreeVisitor<String>
    {
        private Boolean InRegexSet;

        public override String Visit ( AllMatcher allMatcher )
            => $"( {String.Join ( " ", Array.ConvertAll ( allMatcher.PatternMatchers, pm => this.Visit ( pm ) ) )} )";

        public override String Visit ( AnyMatcher anyMatcher )
        {
            if ( Array.TrueForAll ( anyMatcher.PatternMatchers, matcher => matcher is CharMatcher || matcher is CharRangeMatcher ) )
            {
                this.InRegexSet = true;
                var repr = $"[{String.Join ( "", Array.ConvertAll ( anyMatcher.PatternMatchers, this.Visit ) )}]";
                this.InRegexSet = false;
                return repr;
            }
            else
                return $"( {String.Join ( " | ", Array.ConvertAll ( anyMatcher.PatternMatchers, this.Visit ) )} )";
        }

        public override String Visit ( CharMatcher charMatcher )
        {
            var repr = StringUtilities.GetCharacterRepresentation ( charMatcher.Filter );
            return this.InRegexSet ? repr : $"'{repr}'";
        }

        public override String Visit ( CharRangeMatcher charRangeMatcher )
        {
            var start = charRangeMatcher.Strict ? charRangeMatcher.Start : ( Char ) ( charRangeMatcher.Start + 1 );
            var end = charRangeMatcher.Strict ? charRangeMatcher.End : ( Char ) ( charRangeMatcher.End - 1 );
            var startRepr = StringUtilities.GetCharacterRepresentation ( start );
            var endRepr = StringUtilities.GetCharacterRepresentation ( end );
            var repr = $"{startRepr}-{endRepr}";
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        public override String Visit ( FilterFuncMatcher filterFuncMatcher )
            => throw new NotSupportedException ( "FilterFuncMatchers are not implemented in expressions." );

        public override String Visit ( MultiCharMatcher multiCharMatcher )
        {
            var repr = String.Join ( "", Array.ConvertAll ( multiCharMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) );
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        public override String Visit ( RulePlaceholder rulePlaceholder )
            => rulePlaceholder.Name;

        public override String Visit ( StringMatcher stringMatcher )
            => $"'{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}'";

        public override String Visit ( IgnoreMatcher ignoreMatcher )
        {
            return ignoreMatcher.PatternMatcher is MarkerMatcher markerMatcher
                ? $"im:({this.Visit ( markerMatcher.PatternMatcher )})"
                : $"i:({this.Visit ( ignoreMatcher.PatternMatcher )})";
        }

        public override String Visit ( JoinMatcher joinMatcher )
            => $"j:({this.Visit ( joinMatcher.PatternMatcher )})";

        public override String Visit ( NegatedMatcher negatedMatcher )
            => $"!({this.Visit ( negatedMatcher.PatternMatcher )})";

        public override String Visit ( OptionalMatcher optionalMatcher )
            => $"({this.Visit ( optionalMatcher.PatternMatcher )})?";

        public override String Visit ( RepeatedMatcher repeatedMatcher )
        {
            if ( repeatedMatcher.Minimum == 0 && repeatedMatcher.Maximum == Int32.MaxValue )
                return $"({this.Visit ( repeatedMatcher.PatternMatcher )})*";
            else if ( repeatedMatcher.Minimum == 1 && repeatedMatcher.Maximum == Int32.MaxValue )
                return $"({this.Visit ( repeatedMatcher.PatternMatcher )})+";
            else
                return $"({this.Visit ( repeatedMatcher.PatternMatcher )}){{{repeatedMatcher.Minimum}, {repeatedMatcher.Maximum}}}";
        }

        public override String Visit ( RuleWrapper ruleWrapper )
            => this.Visit ( ruleWrapper.PatternMatcher );

        public override String Visit ( MarkerMatcher markerMatcher )
            => $"m:({this.Visit ( markerMatcher.PatternMatcher )})";

        public override String Visit ( EOFMatcher eofMatcher )
            => "EOF";
    }
}
