﻿using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    public class EBNFReconstructor : IMatcherTreeVisitor<String>
    {
        public String Visit ( SequentialMatcher SequentialMatcher ) => $"({String.Join ( ", ", Array.ConvertAll ( SequentialMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";

        public String Visit ( AlternatedMatcher AlternatedMatcher ) => $"({String.Join ( " | ", Array.ConvertAll ( AlternatedMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";

        public String Visit ( CharMatcher charMatcher ) => $"'{charMatcher.Filter}'";

        public String Visit ( RangeMatcher RangeMatcher )
        {
            var start = RangeMatcher.Range.Start - 1;
            var end = RangeMatcher.Range.End + 1;
            return $"? interval (0x{start:X2}, 0x{end:X2}) ?";
        }

        public String Visit ( FilterFuncMatcher filterFuncMatcher ) => $"? {filterFuncMatcher.FullFilterName} ?";

        public String Visit ( CharListMatcher CharListMatcher ) => $"( '{String.Join ( "' | '", CharListMatcher.Whitelist )}' )";

        public String Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder.Name;

        public String Visit ( StringMatcher stringMatcher ) => $"'{stringMatcher.StringFilter}'";

        public String Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        public String Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        public String Visit ( NegatedMatcher negatedMatcher ) => $"-{negatedMatcher.PatternMatcher.Accept ( this )}";

        public String Visit ( OptionalMatcher optionalMatcher ) => $"[{optionalMatcher.PatternMatcher.Accept ( this )}]";

        public String Visit ( RepeatedMatcher repeatedMatcher )
        {
            var list = new List<String> ( );
            for ( var i = 0; i < repeatedMatcher.Range.Start; i++ )
                list.Add ( repeatedMatcher.PatternMatcher.Accept ( this ) );
            list.Add ( $"{repeatedMatcher.PatternMatcher.Accept ( this )} * {repeatedMatcher.Range.End}" );
            return $"( { String.Join ( ", ", list ) } )";
        }

        public String Visit ( RuleWrapper ruleWrapper ) => $"{ruleWrapper.Name} = {ruleWrapper.PatternMatcher.Accept ( this )};";

        public String Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        public String Visit ( EOFMatcher eofMatcher ) => "EOF";

        public String Visit ( SavingMatcher savingMatcher ) => $"{savingMatcher.PatternMatcher.Accept ( this )} ? then save result to memslot '{savingMatcher.SaveName}' ?";

        public String Visit ( LoadingMatcher loadingMatcher ) => $"? match content on memslot '{loadingMatcher.SaveName}' ?";
    }
}
