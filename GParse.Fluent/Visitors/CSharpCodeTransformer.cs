using System;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    /// <summary>
    /// Transforms a matcher tree into C# code
    /// </summary>
    public class CSharpCodeTransformer : IMatcherTreeVisitor<String>
    {
        private static String FormatChar ( Char ch ) =>
            ( Char.IsLetterOrDigit ( ch ) || Char.IsPunctuation ( ch ) || Char.IsSymbol ( ch ) || ch == ' ' )
                ? $"'{ch}'" : $"'\\x{( Int32 ) ch:X}'";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="sequentialMatcher"></param>
        /// <returns></returns>
        public String Visit ( SequentialMatcher sequentialMatcher ) => $"new SequentialMatcher ( {String.Join ( ", ", Array.ConvertAll ( sequentialMatcher.PatternMatchers, matcher => matcher.Accept ( this ) ) )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="alternatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( AlternatedMatcher alternatedMatcher ) => $"new AlternatedMatcher ( {String.Join ( ", ", Array.ConvertAll ( alternatedMatcher.PatternMatchers, matcher => matcher.Accept ( this ) ) )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharMatcher charMatcher ) => $"new CharMatcher ( {FormatChar ( charMatcher.Filter )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rangeMatcher"></param>
        /// <returns></returns>
        public String Visit ( RangeMatcher rangeMatcher ) => $"new RangeMatcher ( {FormatChar ( rangeMatcher.Range.Start )}, {FormatChar ( rangeMatcher.Range.End )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public String Visit ( EOFMatcher eofMatcher ) => "new EOFMatcher ( )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public String Visit ( FilterFuncMatcher filterFuncMatcher ) =>
            filterFuncMatcher.Filter.Target != null
                ? $"new FilterFuncMatcher ( {filterFuncMatcher.FullFilterName} )"
                : throw new NotSupportedException ( );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public String Visit ( IgnoreMatcher ignoreMatcher ) => $"new IgnoreMatcher ( {ignoreMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public String Visit ( JoinMatcher joinMatcher ) => $"new JoinMatcher ( {joinMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public String Visit ( MarkerMatcher markerMatcher ) => $"new MarkerMatcher ( {markerMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charListMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharListMatcher charListMatcher ) => $"new CharListMatcher ( {String.Join ( ", ", Array.ConvertAll ( charListMatcher.Whitelist, ch => FormatChar ( ch ) ) )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( NegatedMatcher negatedMatcher ) => $"new NegatedMatcher ( {negatedMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public String Visit ( OptionalMatcher optionalMatcher ) => $"new OptionalMatcher ( {optionalMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( RepeatedMatcher repeatedMatcher ) => $"new RepeatedMatcher ( {repeatedMatcher.PatternMatcher.Accept ( this )}, new Common.Math.Range<UInt32> ( {repeatedMatcher.Range.Start}, {repeatedMatcher.Range.End} ) )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public String Visit ( RulePlaceholder rulePlaceholder ) => $"new RulePlaceholder ( \"{rulePlaceholder.Name.Replace ( "\"", "\\\"" )}\" )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public String Visit ( RuleWrapper ruleWrapper ) => $"new RuleWrapper ( {ruleWrapper.PatternMatcher.Accept ( this )}, \"{ruleWrapper.Name.Replace ( "\"", "\\\"" )}\" )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public String Visit ( StringMatcher stringMatcher ) => $"new StringMatcher ( \"{stringMatcher.StringFilter.Replace ( "\"", "\\\"" )}\" )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public String Visit ( SavingMatcher savingMatcher ) => $"new SavingMatcher ( \"{savingMatcher.SaveName.Replace ( "\"", "\\\"" )}\", {savingMatcher.PatternMatcher.Accept ( this )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public String Visit ( LoadingMatcher loadingMatcher ) => $"new LoadingMatcher ( \"{loadingMatcher.SaveName.Replace ( "\"", "\\\"" )}\" )";
    }
}
