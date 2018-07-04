using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Verbose.MathUtils;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Visitors;

namespace GParse.Verbose.Optimization
{
    public class MatchTreeOptimizer : IMatcherTreeVisitor<BaseMatcher>
    {
        [Flags]
        private enum RemoveTypes
        {
            None = 0b00,
            Ignores = 0b01,
            Joins = 0b10,
            Cosmetics = Ignores & Joins
        }

        private RemoveTypes Remove = RemoveTypes.None;
        private readonly TreeOptimizerOptions OptimizerOptions;

        public MatchTreeOptimizer ( TreeOptimizerOptions? optimizerOptions = null )
        {
            this.OptimizerOptions = optimizerOptions ?? TreeOptimizerOptions.All;
        }

        #region Basic Matchers (can't be optimized)

        public BaseMatcher Visit ( CharMatcher charMatcher ) => charMatcher;

        public BaseMatcher Visit ( CharRangeMatcher charRangeMatcher ) => charRangeMatcher;

        public BaseMatcher Visit ( FilterFuncMatcher filterFuncMatcher ) => filterFuncMatcher;

        public BaseMatcher Visit ( MultiCharMatcher multiCharMatcher ) => multiCharMatcher;

        public BaseMatcher Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder;

        public BaseMatcher Visit ( StringMatcher stringMatcher ) => stringMatcher;

        #endregion Basic Matchers (can't be optimized)

        #region AllMatcher Optimizations

        /// <summary>
        /// Flattens all <see cref="AllMatcher" /> into a single one
        /// </summary>
        /// <param name="allMatcher"></param>
        /// <returns></returns>
        private static AllMatcher FlattenAllMatchers ( AllMatcher allMatcher )
        {
            var matchers = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in allMatcher.PatternMatchers )
            {
                if ( matcher is AllMatcher subAllMatcher )
                    matchers.AddRange ( subAllMatcher.PatternMatchers );
                else
                    matchers.Add ( matcher );
            }
            return new AllMatcher ( matchers.ToArray ( ) );
        }

        /// <summary>
        /// Join all sequential <see cref="CharMatcher" /> and
        /// <see cref="StringMatcher" /> into a single
        /// <see cref="StringMatcher" /> in
        /// <see cref="AllMatcher" /> components
        /// </summary>
        /// <param name="allMatcher"></param>
        /// <returns></returns>
        private static AllMatcher StringifyComponents ( AllMatcher allMatcher )
        {
            var bufferList = new List<String> ( );
            var matcherList = new List<BaseMatcher> ( );

            foreach ( BaseMatcher matcher in allMatcher.PatternMatchers )
            {
                if ( matcher is IStringContainerMatcher stringContainerMatcher )
                {
                    bufferList.Add ( stringContainerMatcher.StringFilter );
                }
                else
                {
                    // Flush the stringification buffer
                    if ( bufferList.Count > 0 )
                    {
                        matcherList.Add ( new StringMatcher ( String.Join ( "", bufferList ) ) );
                        bufferList.Clear ( );
                    }
                    matcherList.Add ( matcher );
                }
            }
            // Flush the stringification buffer
            if ( bufferList.Count > 0 )
            {
                matcherList.Add ( new StringMatcher ( String.Join ( "", bufferList ) ) );
                bufferList.Clear ( );
            }
            return new AllMatcher ( matcherList.ToArray ( ) );
        }

        public BaseMatcher Visit ( AllMatcher allMatcher )
        {
            // flatten the tree if the user wishes so
            if ( ( this.OptimizerOptions.AllMatcher & TreeOptimizerOptions.AllMatcherFlags.Flatten ) != 0 )
                allMatcher = FlattenAllMatchers ( allMatcher );

            // Optimize all inner matchers at the start
            for ( var i = 0; i < allMatcher.PatternMatchers.Length; i++ )
                allMatcher.PatternMatchers[i] = allMatcher.PatternMatchers[i].Accept ( this );

            // flatten the tree if the user wishes so
            if ( ( this.OptimizerOptions.AllMatcher & TreeOptimizerOptions.AllMatcherFlags.Flatten ) != 0 )
                allMatcher = FlattenAllMatchers ( allMatcher );

            // stringify matchers if the user wishes so
            if ( ( this.OptimizerOptions.AllMatcher & TreeOptimizerOptions.AllMatcherFlags.Stringify ) != 0 )
                allMatcher = StringifyComponents ( allMatcher );

            // and also optimize all matchers at the end
            for ( var i = 0; i < allMatcher.PatternMatchers.Length; i++ )
                allMatcher.PatternMatchers[i] = allMatcher.PatternMatchers[i].Accept ( this );

            // returns the allmatcher if there's more than one
            // matcher inside it, otherwise return the lone matcher
            return allMatcher.PatternMatchers.Length > 1 ? allMatcher : allMatcher.PatternMatchers[0];
        }

        #endregion AllMatcher Optimizations

        #region AnyMatcher Optimizations

        /// <summary>
        /// Flattens all <see cref="AnyMatcher" /> into a single one
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] FlattenAnyMatchers ( BaseMatcher[] array )
        {
            var matchers = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in array )
            {
                if ( matcher is AnyMatcher subAnyMatcher )
                    matchers.AddRange ( subAnyMatcher.PatternMatchers );
                else
                    matchers.Add ( matcher );
            }
            return matchers.ToArray ( );
        }

        /// <summary>
        /// Removes duplicates from the <paramref name="array" /> array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] RemoveDuplicates ( BaseMatcher[] array )
            => new HashSet<BaseMatcher> ( array ).ToArray ( );

        /// <summary>
        /// Gets sequences of characters and transforms them into
        /// ranges for performance
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] RangifyMatchers ( BaseMatcher[] array )
        {
            var charList = new List<Char> ( );
            var matcherList = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in array )
            {
                if ( matcher is CharMatcher charMatcher )
                    charList.Add ( charMatcher.Filter );
                else if ( matcher is MultiCharMatcher multiCharMatcher )
                    charList.AddRange ( multiCharMatcher.Whitelist );
                else
                    matcherList.Add ( matcher );
            }

            charList.Sort ( );
            var charQueue = new Queue<Char> ( charList );
            while ( charQueue.Count > 0 )
            {
                var start = charQueue.Dequeue ( );
                var end = start;

                while ( charQueue.Count > 0 && ( charQueue.Peek ( ) - end ) == 1 )
                    end = charQueue.Dequeue ( );

                if ( start != end )
                    matcherList.Add ( new CharRangeMatcher ( start, end ) );
                else
                    matcherList.Add ( new CharMatcher ( start ) );
            }

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins all <see cref="CharMatcher" /> and
        /// <see cref="MultiCharMatcher" /> into a single <see cref="MultiCharMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] JoinCharBasedMatchers ( BaseMatcher[] array )
        {
            var charList = new List<Char> ( );
            var matcherList = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in array )
            {
                if ( matcher is CharMatcher charMatcher )
                    charList.Add ( charMatcher.Filter );
                else if ( matcher is MultiCharMatcher multiCharMatcher )
                    charList.AddRange ( multiCharMatcher.Whitelist );
                else
                    matcherList.Add ( matcher );
            }

            if ( charList.Count > 1 )
                matcherList.Add ( new MultiCharMatcher ( charList.ToArray ( ) ) );
            else if ( charList.Count > 0 )
                matcherList.Add ( new CharMatcher ( charList[0] ) );

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins multiple intersecting ranges into bigger
        /// spanning <see cref="CharRangeMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] JoinIntersectingRanges ( BaseMatcher[] array )
        {
            var idxs = new List<BaseMatcher> ( array );

            for ( var i1 = 0; i1 < idxs.Count; i1++ )
            {
                if ( !( idxs[i1] is CharRangeMatcher matcher1 ) )
                    continue;
                Range range1 = matcher1.Range;
                for ( var i2 = i1 + 1; i2 < idxs.Count; i2++ )
                {
                    if ( !( idxs[i2] is CharRangeMatcher matcher2 ) )
                        continue;
                    Range range2 = matcher2.Range;
                    if ( range1.IntersectsWith ( range2 ) || range1.IsNeighbourOf ( range2 ) )
                    {
                        idxs.RemoveAt ( i2 );
                        range1 = range1.JoinWith ( range2 );
                        i2 -= 1;
                    }
                }
                if ( range1 != matcher1.Range )
                    idxs[i1] = new CharRangeMatcher ( range1 );
            }

            return idxs.ToArray ( );
        }

        /// <summary>
        /// Removes <see cref="Char" /> from
        /// <see cref="CharMatcher" /> and
        /// <see cref="MultiCharMatcher" /> that would be matched
        /// by other <see cref="CharRangeMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] RemoveIntersectingChars ( BaseMatcher[] array )
        {
            CharRangeMatcher[] ranges = Array.ConvertAll (
                Array.FindAll ( array, matcher => matcher is CharRangeMatcher ),
                matcher => matcher as CharRangeMatcher
            );
            var list = new List<BaseMatcher> ( array.Length );
            foreach ( BaseMatcher matcher in array )
                if ( !( matcher is CharMatcher cmatcher && Array.Exists ( ranges, range => range.Range.ValueIn ( cmatcher.Filter ) ) ) )
                    list.Add ( matcher );
            return list.ToArray ( );
        }

        /// <summary>
        /// Optimizes an <see cref="AnyMatcher" />
        /// </summary>
        /// <param name="anyMatcher"></param>
        /// <returns></returns>
        public BaseMatcher Visit ( AnyMatcher anyMatcher )
        {
            BaseMatcher[] array = anyMatcher.PatternMatchers;
            var rec = new ExpressionReconstructor ( );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.Flatten ) != 0 )
                array = FlattenAnyMatchers ( array );

            for ( var i = 0; i < array.Length; i++ )
                array[i] = array[i].Accept ( this );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.Flatten ) != 0 )
                array = FlattenAnyMatchers ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.RemoveDuplicates ) != 0 )
                array = RemoveDuplicates ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.RangifyMatchers ) != 0 )
                array = RangifyMatchers ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.JoinIntersectingRanges ) != 0 )
                array = JoinIntersectingRanges ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.RemoveIntersectingChars ) != 0 )
                array = RemoveIntersectingChars ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.JoinCharBasedMatchers ) != 0 )
                array = JoinCharBasedMatchers ( array );

            for ( var i = 0; i < array.Length; i++ )
                array[i] = array[i].Accept ( this );

            return array.Length > 1 ? new AnyMatcher ( array ) : array[0];
        }

        #endregion AnyMatcher Optimizations

        public BaseMatcher Visit ( IgnoreMatcher ignoreMatcher )
        {
            if ( ( this.Remove & RemoveTypes.Ignores ) != 0 )
                return ignoreMatcher.PatternMatcher.Accept ( this );

            this.Remove = RemoveTypes.None;
            if ( ( this.OptimizerOptions.IgnoreMatcher & TreeOptimizerOptions.IgnoreMatcherFlags.RemoveNestedIgores ) != 0 )
                this.Remove &= RemoveTypes.Ignores;
            if ( ( this.OptimizerOptions.IgnoreMatcher & TreeOptimizerOptions.IgnoreMatcherFlags.RemoveNestedJoins ) != 0 )
                this.Remove &= RemoveTypes.Joins;

            try
            {
                return new IgnoreMatcher ( ignoreMatcher.PatternMatcher.Accept ( this ) );
            }
            finally
            {
                this.Remove = RemoveTypes.None;
            }
        }

        public BaseMatcher Visit ( JoinMatcher joinMatcher )
        {
            if ( ( this.Remove & RemoveTypes.Joins ) != 0 )
                return joinMatcher.PatternMatcher.Accept ( this );

            this.Remove = RemoveTypes.None;
            if ( ( this.OptimizerOptions.JoinMatcher & TreeOptimizerOptions.JoinMatcherFlags.IgnoreInnerJoins ) != 0 )
                this.Remove &= RemoveTypes.Joins;

            try
            {
                return new JoinMatcher ( joinMatcher.PatternMatcher.Accept ( this ) );
            }
            finally
            {
                this.Remove = RemoveTypes.None;
            }
        }

        public BaseMatcher Visit ( NegatedMatcher negatedMatcher )
        {
            // !!a ≡ a
            if ( ( this.OptimizerOptions.NegatedMatcher & TreeOptimizerOptions.NegatedMatcherFlags.RemoveDoubleNegations ) != 0
                && negatedMatcher.PatternMatcher is NegatedMatcher matcher )
                return this.Visit ( new IgnoreMatcher ( matcher.PatternMatcher.Accept ( this ) ) );
            return new NegatedMatcher ( negatedMatcher.PatternMatcher.Accept ( this ) );
        }

        public BaseMatcher Visit ( OptionalMatcher optionalMatcher )
        {
            BaseMatcher m = optionalMatcher.PatternMatcher.Accept ( this );
            // expr{s, e}? ≡ expr{0, e} | s < 2
            if ( ( this.OptimizerOptions.OptionalMatcher & TreeOptimizerOptions.OptionalMatcherFlags.JoinWithNestedRepeatMatcher ) != 0
                && m is RepeatedMatcher repeated && repeated.Range.Start < 2 )
                return this.Visit ( new RepeatedMatcher ( repeated.PatternMatcher, new Range ( 0, repeated.Range.End ) ) );

            return new OptionalMatcher ( m );
        }

        private static UInt32 MultiplyValuesClamped ( UInt32 lhs, UInt32 rhs )
        {
            var res = lhs * rhs;
            return res % lhs != 0 || res % rhs != 0 ? UInt32.MaxValue : res;
        }

        // Optimizing logic for this is too annoying
        public BaseMatcher Visit ( RepeatedMatcher outerRepeat )
            => outerRepeat;

        public BaseMatcher Visit ( RuleWrapper ruleWrapper )
            => new RuleWrapper ( ruleWrapper.PatternMatcher.Accept ( this ), ruleWrapper.Name );

        public BaseMatcher Visit ( MarkerMatcher markerMatcher )
            => new MarkerMatcher ( markerMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( EOFMatcher eofMatcher )
            => eofMatcher;
    }
}
