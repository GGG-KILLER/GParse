﻿using System;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Utilities;

namespace GParse.Fluent.Visitors
{
    public class ExpressionReconstructor : IMatcherTreeVisitor<String>
    {
        private Boolean InRegexSet;

        public String Visit ( SequentialMatcher SequentialMatcher )
            => $"( {String.Join ( " ", Array.ConvertAll ( SequentialMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )} )";

        public String Visit ( AlternatedMatcher AlternatedMatcher )
        {
            if ( Array.TrueForAll ( AlternatedMatcher.PatternMatchers, matcher => matcher is CharMatcher || matcher is RangeMatcher
                || matcher is CharListMatcher ) )
            {
                this.InRegexSet = true;
                var repr = $"[{String.Join ( "", Array.ConvertAll ( AlternatedMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )}]";
                this.InRegexSet = false;
                return repr;
            }
            else
                return $"( {String.Join ( " | ", Array.ConvertAll ( AlternatedMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )} )";
        }

        public String Visit ( CharMatcher charMatcher )
        {
            var repr = StringUtilities.GetCharacterRepresentation ( charMatcher.Filter );
            return this.InRegexSet ? repr : $"'{repr}'";
        }

        public String Visit ( RangeMatcher RangeMatcher )
        {
            var start = RangeMatcher.Range.Start;
            var end = RangeMatcher.Range.End;
            var startRepr = StringUtilities.GetCharacterRepresentation ( ( Char ) start );
            var endRepr = StringUtilities.GetCharacterRepresentation ( ( Char ) end );
            var repr = $"{startRepr}-{endRepr}";
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        public String Visit ( FilterFuncMatcher filterFuncMatcher )
            => throw new NotSupportedException ( "FilterFuncMatchers are not implemented in expressions." );

        public String Visit ( CharListMatcher CharListMatcher )
        {
            var repr = String.Join ( "", Array.ConvertAll ( CharListMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) );
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
            if ( repeatedMatcher.Range.Start == 0 && repeatedMatcher.Range.End == UInt32.MaxValue )
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )})*";
            else if ( repeatedMatcher.Range.Start == 1 && repeatedMatcher.Range.End == UInt32.MaxValue )
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )})+";
            else if ( repeatedMatcher.Range.IsSingle )
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )}){{{repeatedMatcher.Range.Start}}}";
            else
                return $"({repeatedMatcher.PatternMatcher.Accept ( this )}){{{repeatedMatcher.Range.Start}, {repeatedMatcher.Range.End}}}";
        }

        public String Visit ( RuleWrapper ruleWrapper )
            => ruleWrapper.PatternMatcher.Accept ( this );

        public String Visit ( MarkerMatcher markerMatcher )
            => $"m:({markerMatcher.PatternMatcher.Accept ( this )})";

        public String Visit ( EOFMatcher eofMatcher )
            => "EOF";

        public String Visit ( SavingMatcher savingMatcher )
            => $"s:{savingMatcher.SaveName}:({savingMatcher.PatternMatcher.Accept ( this )})";

        public String Visit ( LoadingMatcher loadingMatcher )
            => $"l:{loadingMatcher.SaveName}";
    }
}