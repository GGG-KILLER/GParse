using System;
using GParse.Utilities;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    /// <summary>
    /// Transforms a matcher tree into an expression
    /// </summary>
    public class ExpressionReconstructor : IMatcherTreeVisitor<String>
    {
        private Boolean InRegexSet;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        public String Visit ( SequentialMatcher SequentialMatcher )
            => $"( {String.Join ( " ", Array.ConvertAll ( SequentialMatcher.PatternMatchers, pm => pm.Accept ( this ) ) )} )";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharMatcher charMatcher )
        {
            var repr = StringUtilities.GetCharacterRepresentation ( charMatcher.Filter );
            return this.InRegexSet ? repr : $"'{repr}'";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        /// <returns></returns>
        public String Visit ( RangeMatcher RangeMatcher )
        {
            var start = RangeMatcher.Range.Start;
            var end = RangeMatcher.Range.End;
            var startRepr = StringUtilities.GetCharacterRepresentation (    start );
            var endRepr = StringUtilities.GetCharacterRepresentation (    end );
            var repr = $"{startRepr}-{endRepr}";
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public String Visit ( FilterFuncMatcher filterFuncMatcher )
            => throw new NotSupportedException ( "FilterFuncMatchers are not implemented in expressions." );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="CharListMatcher"></param>
        /// <returns></returns>
        public String Visit ( CharListMatcher CharListMatcher )
        {
            var repr = String.Join ( "", Array.ConvertAll ( CharListMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) );
            return this.InRegexSet ? repr : $"[{repr}]";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public String Visit ( RulePlaceholder rulePlaceholder )
            => rulePlaceholder.Name;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public String Visit ( StringMatcher stringMatcher )
            => $"'{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}'";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public String Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher is MarkerMatcher markerMatcher
                ? $"im:({markerMatcher.PatternMatcher.Accept ( this )})"
                : $"i:({ignoreMatcher.PatternMatcher.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public String Visit ( JoinMatcher joinMatcher )
            => $"j:({joinMatcher.PatternMatcher.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public String Visit ( NegatedMatcher negatedMatcher )
            => $"!({negatedMatcher.PatternMatcher.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public String Visit ( OptionalMatcher optionalMatcher )
            => $"({optionalMatcher.PatternMatcher.Accept ( this )})?";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public String Visit ( RuleWrapper ruleWrapper )
            => ruleWrapper.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public String Visit ( MarkerMatcher markerMatcher )
            => $"m:({markerMatcher.PatternMatcher.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public String Visit ( EOFMatcher eofMatcher )
            => "EOF";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public String Visit ( SavingMatcher savingMatcher )
            => $"s:{savingMatcher.SaveName}:({savingMatcher.PatternMatcher.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public String Visit ( LoadingMatcher loadingMatcher )
            => $"l:{loadingMatcher.SaveName}";
    }
}
