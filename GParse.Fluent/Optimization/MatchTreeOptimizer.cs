using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Common.Math;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Utilities;

namespace GParse.Fluent.Optimization
{
    public class MatchTreeOptimizer : IMatcherTreeVisitor<BaseMatcher>
    {
        #region Basic Matchers (can't be optimized)

        public BaseMatcher Visit ( CharMatcher charMatcher ) => charMatcher;

        public BaseMatcher Visit ( RangeMatcher RangeMatcher ) => RangeMatcher;

        public BaseMatcher Visit ( FilterFuncMatcher filterFuncMatcher ) => filterFuncMatcher;

        public BaseMatcher Visit ( CharListMatcher CharListMatcher ) => CharListMatcher;

        public BaseMatcher Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder;

        public BaseMatcher Visit ( StringMatcher stringMatcher ) => stringMatcher;

        #endregion Basic Matchers (can't be optimized)

        #region Non-basic non-optimizable matchers

        public BaseMatcher Visit ( NegatedMatcher negatedMatcher ) => new NegatedMatcher ( negatedMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( RuleWrapper ruleWrapper )
            => new RuleWrapper ( ruleWrapper.PatternMatcher.Accept ( this ), ruleWrapper.Name );

        public BaseMatcher Visit ( MarkerMatcher markerMatcher )
            => new MarkerMatcher ( markerMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( EOFMatcher eofMatcher )
            => eofMatcher;

        public BaseMatcher Visit ( SavingMatcher savingMatcher )
            => new SavingMatcher ( savingMatcher.SaveName, savingMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( LoadingMatcher loadingMatcher )
            => loadingMatcher;

        public BaseMatcher Visit ( SequentialMatcher SequentialMatcher )
            => new SequentialMatcher ( Array.ConvertAll ( SequentialMatcher.PatternMatchers, m => m.Accept ( this ) ) );

        public BaseMatcher Visit ( IgnoreMatcher ignoreMatcher )
            => new IgnoreMatcher ( ignoreMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( JoinMatcher joinMatcher )
            => new JoinMatcher ( joinMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( OptionalMatcher optionalMatcher )
            => new OptionalMatcher ( optionalMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( RepeatedMatcher repeatedMatcher )
            => new RepeatedMatcher ( repeatedMatcher.PatternMatcher.Accept ( this ), repeatedMatcher.Range );

        #endregion Non-basic non-optimizable matchers

        #region AlternatedMatcher Optimizations

        /// <summary>
        /// Gets sequences of characters and transforms them into
        /// ranges for performance
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private BaseMatcher[] RangifyMatchers ( BaseMatcher[] array )
        {
            var charSet = new SortedSet<Char> ( );
            var matcherList = new NoDuplicatesList<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in array )
            {
                if ( matcher is CharMatcher charMatcher )
                    charSet.Add ( charMatcher.Filter );
                else if ( matcher is CharListMatcher CharListMatcher )
                    charSet.UnionWith ( CharListMatcher.Whitelist );
                else
                    matcherList.Add ( matcher );
            }

            var charQueue = new Queue<Char> ( charSet );
            while ( charQueue.Count > 0 )
            {
                var start = charQueue.Dequeue ( );
                var end = start;

                while ( charQueue.Count > 0 && end + 1u == charQueue.Peek ( ) )
                    end = charQueue.Dequeue ( );

                if ( start != end )
                    matcherList.Add ( new RangeMatcher ( start, end ).Accept ( this ) );
                else
                    matcherList.Add ( new CharMatcher ( start ).Accept ( this ) );
            }

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins all <see cref="CharMatcher" /> and
        /// <see cref="CharListMatcher" /> into a single <see cref="CharListMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private BaseMatcher[] JoinCharBasedMatchers ( BaseMatcher[] array )
        {
            var idx = -1;
            var charList = new HashSet<Char> ( );
            var matcherList = new NoDuplicatesList<BaseMatcher> ( );
            for ( var i = 0; i < array.Length; i++ )
            {
                BaseMatcher matcher = array[i];
                if ( matcher is CharMatcher charMatcher )
                {
                    if ( idx == -1 )
                        idx = i;
                    charList.Add ( charMatcher.Filter );
                }
                else if ( matcher is CharListMatcher CharListMatcher )
                {
                    if ( idx == -1 )
                        idx = i;
                    charList.UnionWith ( CharListMatcher.Whitelist );
                }
                else
                    matcherList.Add ( matcher );
            }

            if ( charList.Count > 1 )
                matcherList.Insert ( idx, new CharListMatcher ( charList.ToArray ( ) ).Accept ( this ) );
            else if ( charList.Count > 0 )
                matcherList.Insert ( idx, new CharMatcher ( charList.GetEnumerator ( ).Current ).Accept ( this ) );

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins multiple intersecting ranges into bigger
        /// spanning <see cref="RangeMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private BaseMatcher[] JoinIntersectingRanges ( BaseMatcher[] array )
        {
            var matchers = new NoDuplicatesList<BaseMatcher> ( array.Length );
            var ranges = new Stack<(Range<Char>, Int32)> ( array.Length );

            for ( var i = 0; i < array.Length; i++ )
            {
                BaseMatcher matcher = array[i];
                if ( matcher is RangeMatcher range )
                    ranges.Push ( (range.Range, i) );
                else
                    matchers.Add ( matcher );
            }

            while ( ranges.Count > 0 )
            {
                (Range<Char> range1, Int32 idx) = ranges.Pop ( );
                (Range<Char> range2, _) = ranges.Peek ( );
                if ( range1.IntersectsWith ( range2 ) || range1.IsNeighbourOf ( range2 ) )
                {
                    ranges.Pop ( );
                    ranges.Push ( (new Range<Char> ( ( Char ) Math.Min ( range1.Start, range2.Start ), ( Char ) Math.Max ( range1.End, range2.End ) ), idx) );
                }
                else
                    matchers.Insert ( idx, new RangeMatcher ( range1 ).Accept ( this ) );
            }

            return matchers.ToArray ( );
        }

        /// <summary>
        /// Removes <see cref="Char" /> from
        /// <see cref="CharMatcher" /> and
        /// <see cref="CharListMatcher" /> that would be matched
        /// by other <see cref="RangeMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] RemoveIntersectingChars ( BaseMatcher[] array )
        {
            RangeMatcher[] ranges = Array.ConvertAll (
                Array.FindAll ( array, matcher => matcher is RangeMatcher ),
                matcher => matcher as RangeMatcher
            );
            var list = new NoDuplicatesList<BaseMatcher> ( array.Length );
            foreach ( BaseMatcher matcher in array )
            {
                if ( !( matcher is CharMatcher cmatcher && Array.Exists ( ranges, range => range.Range.ValueIn ( cmatcher.Filter ) ) ) )
                    list.Add ( matcher );
            }
            return list.ToArray ( );
        }

        /// <summary>
        /// Optimizes an <see cref="AlternatedMatcher" />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public BaseMatcher Visit ( AlternatedMatcher AlternatedMatcher )
        {
            BaseMatcher[] array = this.JoinCharBasedMatchers (
                RemoveIntersectingChars (
                    this.JoinIntersectingRanges (
                        this.RangifyMatchers (
                            Array.ConvertAll ( AlternatedMatcher.PatternMatchers, elem => elem.Accept ( this ) ) ) ) ) );

            return array.Length > 1 ? new AlternatedMatcher ( array ) : array[0];
        }

        #endregion AlternatedMatcher Optimizations
    }
}
