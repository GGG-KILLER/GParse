using System;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    public class CSharpCodeTransformer : IMatcherTreeVisitor<String>
    {
        private static String FormatChar ( Char ch ) =>
            ( Char.IsLetterOrDigit ( ch ) || Char.IsPunctuation ( ch ) || Char.IsSymbol ( ch ) || ch == ' ' )
                ? $"'{ch}'" : $"'\\x{( Int32 ) ch:X}'";

        public String Visit ( SequentialMatcher sequentialMatcher ) => $"new SequentialMatcher ( {String.Join ( ", ", Array.ConvertAll ( sequentialMatcher.PatternMatchers, matcher => matcher.Accept ( this ) ) )} )";

        public String Visit ( AlternatedMatcher alternatedMatcher ) => $"new AlternatedMatcher ( {String.Join ( ", ", Array.ConvertAll ( alternatedMatcher.PatternMatchers, matcher => matcher.Accept ( this ) ) )} )";

        public String Visit ( CharMatcher charMatcher ) => $"new CharMatcher ( {FormatChar ( charMatcher.Filter )} )";

        public String Visit ( RangeMatcher rangeMatcher ) => $"new RangeMatcher ( {FormatChar ( rangeMatcher.Range.Start )}, {FormatChar ( rangeMatcher.Range.End )} )";

        public String Visit ( EOFMatcher eofMatcher ) => "new EOFMatcher ( )";

        public String Visit ( FilterFuncMatcher filterFuncMatcher ) =>
            filterFuncMatcher.Filter.Target != null
                ? $"new FilterFuncMatcher ( {filterFuncMatcher.FullFilterName} )"
                : throw new NotSupportedException ( );

        public String Visit ( IgnoreMatcher ignoreMatcher ) => $"new IgnoreMatcher ( {ignoreMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( JoinMatcher joinMatcher ) => $"new JoinMatcher ( {joinMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( MarkerMatcher markerMatcher ) => $"new MarkerMatcher ( {markerMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( CharListMatcher charListMatcher ) => $"new CharListMatcher ( {String.Join ( ", ", Array.ConvertAll ( charListMatcher.Whitelist, ch => FormatChar ( ch ) ) )} )";

        public String Visit ( NegatedMatcher negatedMatcher ) => $"new NegatedMatcher ( {negatedMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( OptionalMatcher optionalMatcher ) => $"new OptionalMatcher ( {optionalMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( RepeatedMatcher repeatedMatcher ) => $"new RepeatedMatcher ( {repeatedMatcher.PatternMatcher.Accept ( this )}, new Common.Math.Range<UInt32> ( {repeatedMatcher.Range.Start}, {repeatedMatcher.Range.End} ) )";

        public String Visit ( RulePlaceholder rulePlaceholder ) => $"new RulePlaceholder ( \"{rulePlaceholder.Name.Replace ( "\"", "\\\"" )}\" )";

        public String Visit ( RuleWrapper ruleWrapper ) => $"new RuleWrapper ( {ruleWrapper.PatternMatcher.Accept ( this )}, \"{ruleWrapper.Name.Replace ( "\"", "\\\"" )}\" )";

        public String Visit ( StringMatcher stringMatcher ) => $"new StringMatcher ( \"{stringMatcher.StringFilter.Replace ( "\"", "\\\"" )}\" )";

        public String Visit ( SavingMatcher savingMatcher ) => $"new SavingMatcher ( \"{savingMatcher.SaveName.Replace ( "\"", "\\\"" )}\", {savingMatcher.PatternMatcher.Accept ( this )} )";

        public String Visit ( LoadingMatcher loadingMatcher ) => $"new LoadingMatcher ( \"{loadingMatcher.SaveName.Replace ( "\"", "\\\"" )}\" )";
    }
}
