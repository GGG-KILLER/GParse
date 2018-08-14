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

        public BaseMatcher Visit ( RangeMatcher RangeMatcher ) => RangeMatcher;

        public BaseMatcher Visit ( FilterFuncMatcher filterFuncMatcher ) => filterFuncMatcher;

        public BaseMatcher Visit ( CharListMatcher CharListMatcher ) => CharListMatcher;

        public BaseMatcher Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder;

        public BaseMatcher Visit ( StringMatcher stringMatcher ) => stringMatcher;

        #endregion Basic Matchers (can't be optimized)

        #region SequentialMatcher Optimizations

        /// <summary>
        /// Flattens all <see cref="SequentialMatcher" /> into a single one
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        private static SequentialMatcher FlattenSequentialMatchers ( SequentialMatcher SequentialMatcher )
        {
            var matchers = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
            {
                if ( matcher is SequentialMatcher subSequentialMatcher )
                    matchers.AddRange ( subSequentialMatcher.PatternMatchers );
                else
                    matchers.Add ( matcher );
            }
            return new SequentialMatcher ( matchers.ToArray ( ) );
        }

        /// <summary>
        /// Join all sequential <see cref="CharMatcher" /> and
        /// <see cref="StringMatcher" /> into a single
        /// <see cref="StringMatcher" /> in
        /// <see cref="SequentialMatcher" /> components
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        private static SequentialMatcher StringifyComponents ( SequentialMatcher SequentialMatcher )
        {
            var bufferList = new List<String> ( );
            var matcherList = new List<BaseMatcher> ( );

            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
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
            return new SequentialMatcher ( matcherList.ToArray ( ) );
        }

        public BaseMatcher Visit ( SequentialMatcher SequentialMatcher )
        {
            // flatten the tree if the user wishes so
            if ( ( this.OptimizerOptions.SequentialMatcher & TreeOptimizerOptions.SequentialMatcherFlags.Flatten ) != 0 )
                SequentialMatcher = FlattenSequentialMatchers ( SequentialMatcher );

            // Optimize all inner matchers at the start
            for ( var i = 0; i < SequentialMatcher.PatternMatchers.Length; i++ )
                SequentialMatcher.PatternMatchers[i] = SequentialMatcher.PatternMatchers[i].Accept ( this );

            // flatten the tree if the user wishes so
            if ( ( this.OptimizerOptions.SequentialMatcher & TreeOptimizerOptions.SequentialMatcherFlags.Flatten ) != 0 )
                SequentialMatcher = FlattenSequentialMatchers ( SequentialMatcher );

            // stringify matchers if the user wishes so
            if ( ( this.OptimizerOptions.SequentialMatcher & TreeOptimizerOptions.SequentialMatcherFlags.Stringify ) != 0 )
                SequentialMatcher = StringifyComponents ( SequentialMatcher );

            // and also optimize all matchers at the end
            for ( var i = 0; i < SequentialMatcher.PatternMatchers.Length; i++ )
                SequentialMatcher.PatternMatchers[i] = SequentialMatcher.PatternMatchers[i].Accept ( this );

            // returns the SequentialMatcher if there's more than one
            // matcher inside it, otherwise return the lone matcher
            return SequentialMatcher.PatternMatchers.Length > 1 ? SequentialMatcher : SequentialMatcher.PatternMatchers[0];
        }

        #endregion SequentialMatcher Optimizations

        #region AlternatedMatcher Optimizations

        /// <summary>
        /// Flattens all <see cref="AlternatedMatcher" /> into a single one
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] FlattenAlternatedMatchers ( BaseMatcher[] array )
        {
            var matchers = new List<BaseMatcher> ( );
            foreach ( BaseMatcher matcher in array )
            {
                if ( matcher is AlternatedMatcher subAlternatedMatcher )
                    matchers.AddRange ( subAlternatedMatcher.PatternMatchers );
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
                else if ( matcher is CharListMatcher CharListMatcher )
                    charList.AddRange ( CharListMatcher.Whitelist );
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
                    matcherList.Add ( new RangeMatcher ( start, end ) );
                else
                    matcherList.Add ( new CharMatcher ( start ) );
            }

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins all <see cref="CharMatcher" /> and
        /// <see cref="CharListMatcher" /> into a single <see cref="CharListMatcher" />
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
                else if ( matcher is CharListMatcher CharListMatcher )
                    charList.AddRange ( CharListMatcher.Whitelist );
                else
                    matcherList.Add ( matcher );
            }

            if ( charList.Count > 1 )
                matcherList.Add ( new CharListMatcher ( charList.ToArray ( ) ) );
            else if ( charList.Count > 0 )
                matcherList.Add ( new CharMatcher ( charList[0] ) );

            return matcherList.ToArray ( );
        }

        /// <summary>
        /// Joins multiple intersecting ranges into bigger
        /// spanning <see cref="RangeMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] JoinIntersectingRanges ( BaseMatcher[] array )
        {
            var idxs = new List<BaseMatcher> ( array );

            for ( var i1 = 0; i1 < idxs.Count; i1++ )
            {
                if ( !( idxs[i1] is RangeMatcher matcher1 ) )
                    continue;
                Range range1 = matcher1.Range;
                for ( var i2 = i1 + 1; i2 < idxs.Count; i2++ )
                {
                    if ( !( idxs[i2] is RangeMatcher matcher2 ) )
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
                    idxs[i1] = new RangeMatcher ( range1 );
            }

            return idxs.ToArray ( );
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
            var list = new List<BaseMatcher> ( array.Length );
            foreach ( BaseMatcher matcher in array )
                if ( !( matcher is CharMatcher cmatcher && Array.Exists ( ranges, range => range.Range.ValueIn ( cmatcher.Filter ) ) ) )
                    list.Add ( matcher );
            return list.ToArray ( );
        }

        /// <summary>
        /// Optimizes an <see cref="AlternatedMatcher" />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public BaseMatcher Visit ( AlternatedMatcher AlternatedMatcher )
        {
            BaseMatcher[] array = AlternatedMatcher.PatternMatchers;
            var rec = new ExpressionReconstructor ( );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.Flatten ) != 0 )
                array = FlattenAlternatedMatchers ( array );

            for ( var i = 0; i < array.Length; i++ )
                array[i] = array[i].Accept ( this );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.Flatten ) != 0 )
                array = FlattenAlternatedMatchers ( array );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.RemoveDuplicates ) != 0 )
                array = RemoveDuplicates ( array );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.RangifyMatchers ) != 0 )
                array = RangifyMatchers ( array );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.JoinIntersectingRanges ) != 0 )
                array = JoinIntersectingRanges ( array );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.RemoveIntersectingChars ) != 0 )
                array = RemoveIntersectingChars ( array );

            if ( ( this.OptimizerOptions.AlternatedMatcher & TreeOptimizerOptions.AlternatedMatcherFlags.JoinCharBasedMatchers ) != 0 )
                array = JoinCharBasedMatchers ( array );

            for ( var i = 0; i < array.Length; i++ )
                array[i] = array[i].Accept ( this );

            return array.Length > 1 ? new AlternatedMatcher ( array ) : array[0];
        }

        #endregion AlternatedMatcher Optimizations

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
            // expr{s, e}? ≡ expr{0, e} | s ≤ 2
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

        public BaseMatcher Visit ( SavingMatcher savingMatcher )
            => new SavingMatcher ( savingMatcher.SaveName, savingMatcher.PatternMatcher.Accept ( this ) );

        public BaseMatcher Visit ( LoadingMatcher loadingMatcher )
            => loadingMatcher;
    }
}
