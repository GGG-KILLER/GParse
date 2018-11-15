using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    /// <summary>
    /// Transforms a matcher tree into EBNF
    /// </summary>
    public class EBNFReconstructor : IMatcherTreeVisitor<String>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        public String Visit ( SequentialMatcher SequentialMatcher ) => $"({String.Join ( ", ", Array.ConvertAll ( SequentialMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( AlternatedMatcher AlternatedMatcher ) => $"({String.Join ( " | ", Array.ConvertAll ( AlternatedMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharMatcher charMatcher ) => $"'{charMatcher.Filter}'";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        /// <returns></returns>
        public String Visit ( RangeMatcher RangeMatcher )
        {
            var start = RangeMatcher.Range.Start - 1;
            var end = RangeMatcher.Range.End + 1;
            return $"? interval (0x{start:X2}, 0x{end:X2}) ?";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public String Visit ( FilterFuncMatcher filterFuncMatcher ) => $"? {filterFuncMatcher.FullFilterName} ?";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="CharListMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharListMatcher CharListMatcher ) => $"( '{String.Join ( "' | '", CharListMatcher.Whitelist )}' )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public String Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder.Name;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public String Visit ( StringMatcher stringMatcher ) => $"'{stringMatcher.StringFilter}'";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public String Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public String Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( NegatedMatcher negatedMatcher ) => $"-{negatedMatcher.PatternMatcher.Accept ( this )}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public String Visit ( OptionalMatcher optionalMatcher ) => $"[{optionalMatcher.PatternMatcher.Accept ( this )}]";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( RepeatedMatcher repeatedMatcher )
        {
            var list = new List<String> ( );
            for ( var i = 0; i < repeatedMatcher.Range.Start; i++ )
                list.Add ( repeatedMatcher.PatternMatcher.Accept ( this ) );
            list.Add ( $"{repeatedMatcher.PatternMatcher.Accept ( this )} * {repeatedMatcher.Range.End}" );
            return $"( { String.Join ( ", ", list ) } )";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public String Visit ( RuleWrapper ruleWrapper ) => $"{ruleWrapper.Name} = {ruleWrapper.PatternMatcher.Accept ( this )};";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public String Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public String Visit ( EOFMatcher eofMatcher ) => "EOF";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public String Visit ( SavingMatcher savingMatcher ) => $"{savingMatcher.PatternMatcher.Accept ( this )} ? then save result to memslot '{savingMatcher.SaveName}' ?";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public String Visit ( LoadingMatcher loadingMatcher ) => $"? match content on memslot '{loadingMatcher.SaveName}' ?";
    }
}
