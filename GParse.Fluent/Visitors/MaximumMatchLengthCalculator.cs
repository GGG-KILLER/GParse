using System;
using System.Collections.Generic;
using GParse.Math;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    /// <summary>
    /// Calculates the maximum length of a match
    /// </summary>
    public class MaximumMatchLengthCalculator<NodeT> : IMatcherTreeVisitor<Int32>
    {
        private readonly Dictionary<BaseMatcher, Int32> LengthCache = new Dictionary<BaseMatcher, Int32> ( );
        private readonly FluentParser<NodeT> Parser;
        private readonly Stack<String> RuleStack = new Stack<String> ( );

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="parser"></param>
        public MaximumMatchLengthCalculator ( FluentParser<NodeT> parser )
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Calculates the maximum length match for a matcher
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public Int32 Calculate ( BaseMatcher matcher )
        {
            if ( !this.LengthCache.ContainsKey ( matcher ) )
            {
                this.RuleStack.Clear ( );
                this.LengthCache[matcher] = matcher.Accept ( this );
            }
            return this.LengthCache[matcher];
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( SequentialMatcher SequentialMatcher )
        {
            var maxes = Array.ConvertAll ( SequentialMatcher.PatternMatchers, pm => pm.Accept ( this ) );
            var max = 0;
            for ( var i = 0; i < maxes.Length; i++ )
                max += maxes[i];
            return max;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( AlternatedMatcher AlternatedMatcher )
        {
            var maxes = Array.ConvertAll ( AlternatedMatcher.PatternMatchers, pm => pm.Accept ( this ) );
            var max = -1;
            for ( var i = 0; i < maxes.Length; i++ )
                if ( maxes[i] > max )
                    max = maxes[i];
            return max;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( CharMatcher charMatcher ) => 1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( RangeMatcher RangeMatcher ) => 1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( FilterFuncMatcher filterFuncMatcher ) => 1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="CharListMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( CharListMatcher CharListMatcher ) => 1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public Int32 Visit ( RulePlaceholder rulePlaceholder ) =>
            this.RuleStack.Contains ( rulePlaceholder.Name ) ? 0 : this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( StringMatcher stringMatcher ) => stringMatcher.StringFilter.Length;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( NegatedMatcher negatedMatcher ) => negatedMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( OptionalMatcher optionalMatcher ) => optionalMatcher.PatternMatcher.Accept ( this );

        // Clamp result of these to UInt32.MaxValue since usually
        // the maximum is that
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( RepeatedMatcher repeatedMatcher )
            => ( Int32 ) SaturatingMath.Multiply ( repeatedMatcher.PatternMatcher.Accept ( this ), repeatedMatcher.Range.End, UInt32.MinValue, Int32.MaxValue );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public Int32 Visit ( RuleWrapper ruleWrapper ) => ruleWrapper.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( EOFMatcher eofMatcher )
            => 0;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( SavingMatcher savingMatcher )
            => savingMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public Int32 Visit ( LoadingMatcher loadingMatcher )
            => 0;
    }
}
