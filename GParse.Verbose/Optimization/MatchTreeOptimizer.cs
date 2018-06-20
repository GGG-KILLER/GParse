using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Optimization
{
    public class MatchTreeOptimizer : MatcherTreeVisitor<BaseMatcher>
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

        public MatchTreeOptimizer ( TreeOptimizerOptions optimizerOptions )
        {
            this.OptimizerOptions = optimizerOptions;
        }

        #region Basic Matchers (can't be optimized)

        public override BaseMatcher Visit ( CharMatcher charMatcher ) => charMatcher;

        public override BaseMatcher Visit ( CharRangeMatcher charRangeMatcher ) => charRangeMatcher;

        public override BaseMatcher Visit ( FilterFuncMatcher filterFuncMatcher ) => filterFuncMatcher;

        public override BaseMatcher Visit ( MultiCharMatcher multiCharMatcher ) => multiCharMatcher;

        public override BaseMatcher Visit ( RulePlaceholder rulePlaceholder ) => rulePlaceholder;

        public override BaseMatcher Visit ( StringMatcher stringMatcher ) => stringMatcher;

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

        public override BaseMatcher Visit ( AllMatcher allMatcher )
        {
            // Optimize all inner matchers at the start
            for ( var i = 0; i < allMatcher.PatternMatchers.Length; i++ )
                allMatcher.PatternMatchers[i] = this.Visit ( allMatcher.PatternMatchers[i] );

            // flatten the tree if the user wishes so
            if ( ( this.OptimizerOptions.AllMatcher & TreeOptimizerOptions.AllMatcherFlags.Flatten ) != 0 )
                allMatcher = FlattenAllMatchers ( allMatcher );

            // stringify matchers if the user wishes so
            if ( ( this.OptimizerOptions.AllMatcher & TreeOptimizerOptions.AllMatcherFlags.Stringify ) != 0 )
                allMatcher = StringifyComponents ( allMatcher );

            // and also optimize all matchers at the end
            for ( var i = 0; i < allMatcher.PatternMatchers.Length; i++ )
                allMatcher.PatternMatchers[i] = this.Visit ( allMatcher.PatternMatchers[i] );

            // returns the allmatcher if there's more than one
            // matcher inside it, otherwise return the lone matcher
            return allMatcher.PatternMatchers.Length > 1 ? allMatcher : allMatcher.PatternMatchers[0];
        }

        #endregion AllMatcher Optimizations

        #region AnyMatcher Optimizations

        /// <summary>
        /// Removes duplicates from the <paramref name="input" /> array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static BaseMatcher[] RemoveDuplicates ( BaseMatcher[] input )
            => new HashSet<BaseMatcher> ( input ).ToArray ( );

        /// <summary>
        /// Joins all <see cref="CharMatcher" /> and
        /// <see cref="MultiCharMatcher" /> into a single <see cref="MultiCharMatcher" />
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static BaseMatcher[] JoinAndSimplifyMatchers ( BaseMatcher[] array )
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

            for ( var i1 = 0; i1 < idxs.Count; i1 += 2 )
            {
                if ( !( idxs[i1] is CharRangeMatcher range1 ) )
                    continue;
                Char s = range1.Start, e = range1.End;
                for ( var i2 = i1 + 2; i2 < idxs.Count; i2 += 2 )
                {
                    if ( !( idxs[i2] is CharRangeMatcher range2 ) )
                        continue;
                    if ( range1.End >= range2.Start || range2.End >= range1.Start )
                    {
                        idxs.RemoveAt ( i2 );
                        s = ( Char ) Math.Min ( s, range2.Start );
                        e = ( Char ) Math.Max ( e, range2.End );
                        i2 -= 1;
                    }
                }
                idxs[i1] = new CharRangeMatcher ( s, e, true );
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
            CharRangeMatcher[] ranges = Array.ConvertAll ( Array.FindAll ( array, m => m is CharRangeMatcher ),
                a => a as CharRangeMatcher );
            var list = new List<BaseMatcher> ( array.Length );
            for ( var i = 0; i < array.Length; i++ )
                if ( !( array[i] is CharMatcher charMatcher )
                        || !Array.Exists ( ranges, range => range.Start < charMatcher.Filter && charMatcher.Filter < range.End ) )
                    list.Add ( array[i] );
            return list.ToArray ( );
        }

        /// <summary>
        /// Optimizes an <see cref="AnyMatcher" />
        /// </summary>
        /// <param name="anyMatcher"></param>
        /// <returns></returns>
        public override BaseMatcher Visit ( AnyMatcher anyMatcher )
        {
            BaseMatcher[] array = anyMatcher.PatternMatchers;

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.RemoveDuplicates ) != 0 )
                array = RemoveDuplicates ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.JoinAndSimplifyCharMatchers ) != 0 )
                array = JoinAndSimplifyMatchers ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.JoinIntersectingRanges ) != 0 )
                array = JoinIntersectingRanges ( array );

            if ( ( this.OptimizerOptions.AnyMatcher & TreeOptimizerOptions.AnyMatcherFlags.RemoveIntersectingChars ) != 0 )
                array = RemoveIntersectingChars ( array );

            return array.Length > 1 ? new AnyMatcher ( array ) : array[0];
        }

        #endregion AnyMatcher Optimizations

        public override BaseMatcher Visit ( IgnoreMatcher ignoreMatcher )
        {
            if ( ( this.Remove & RemoveTypes.Ignores ) != 0 )
                return this.Visit ( ignoreMatcher.PatternMatcher );

            this.Remove = RemoveTypes.None;
            if ( ( this.OptimizerOptions.IgnoreMatcher & TreeOptimizerOptions.IgnoreMatcherFlags.RemoveNestedIgores ) != 0 )
                this.Remove &= RemoveTypes.Ignores;
            if ( ( this.OptimizerOptions.IgnoreMatcher & TreeOptimizerOptions.IgnoreMatcherFlags.RemoveNestedJoins ) != 0 )
                this.Remove &= RemoveTypes.Joins;

            try
            {
                return new IgnoreMatcher ( this.Visit ( ignoreMatcher.PatternMatcher ) );
            }
            finally
            {
                this.Remove = RemoveTypes.None;
            }
        }

        public override BaseMatcher Visit ( JoinMatcher joinMatcher )
        {
            if ( ( this.Remove & RemoveTypes.Joins ) != 0 )
                return this.Visit ( joinMatcher.PatternMatcher );

            this.Remove = RemoveTypes.None;
            if ( ( this.OptimizerOptions.JoinMatcher & TreeOptimizerOptions.JoinMatcherFlags.IgnoreInnerJoins ) != 0 )
                this.Remove &= RemoveTypes.Joins;

            try
            {
                return new JoinMatcher ( this.Visit ( joinMatcher.PatternMatcher ) );
            }
            finally
            {
                this.Remove = RemoveTypes.None;
            }
        }

        public override BaseMatcher Visit ( NegatedMatcher negatedMatcher )
        {
            // !!a ≡ a
            if ( ( this.OptimizerOptions.NegatedMatcher & TreeOptimizerOptions.NegatedMatcherFlags.RemoveDoubleNegations ) != 0
                && negatedMatcher.PatternMatcher is NegatedMatcher matcher )
                return this.Visit ( new IgnoreMatcher ( this.Visit ( matcher.PatternMatcher ) ) );
            return new NegatedMatcher ( this.Visit ( negatedMatcher.PatternMatcher ) );
        }

        public override BaseMatcher Visit ( OptionalMatcher optionalMatcher )
        {
            BaseMatcher m = this.Visit ( optionalMatcher.PatternMatcher );
            // A repeated matcher with a minimum match count of 1
            // or 0 inside an optional matcher is a repeated
            // matcher with a minimum match count of 0
            if ( ( this.OptimizerOptions.OptionalMatcher & TreeOptimizerOptions.OptionalMatcherFlags.JoinWithNestedRepeatMatcher ) != 0
                && m is RepeatedMatcher repeated && repeated.Minimum < 2 )
                return this.Visit ( new RepeatedMatcher ( repeated.PatternMatcher, 0, repeated.Maximum ) );

            return new OptionalMatcher ( m );
        }

        public override BaseMatcher Visit ( RepeatedMatcher repeatedMatcher )
        {
            BaseMatcher subMatcher = this.Visit ( repeatedMatcher.PatternMatcher );
            // Simplify stuff
            if ( subMatcher is RepeatedMatcher subRepeated )
                return new RepeatedMatcher ( subRepeated.PatternMatcher,
                    repeatedMatcher.Minimum * subRepeated.Minimum,
                    repeatedMatcher.Maximum * subRepeated.Maximum );
            else
                return new RepeatedMatcher ( subMatcher, repeatedMatcher.Minimum, repeatedMatcher.Maximum );
        }

        public override BaseMatcher Visit ( RuleWrapper ruleWrapper )
            => new RuleWrapper ( this.Visit ( ruleWrapper.PatternMatcher ), ruleWrapper.Name );

        public override BaseMatcher Visit ( MarkerMatcher markerMatcher )
            => new MarkerMatcher ( this.Visit ( markerMatcher.PatternMatcher ) );

        public override BaseMatcher Visit ( EOFMatcher eofMatcher )
            => eofMatcher;
    }
}
