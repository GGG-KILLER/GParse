using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors
{
    public class ValidExpressionGenerator : IMatcherTreeVisitor<HashSet<String>>
    {
        private static readonly Char[] AllChars;
        private readonly FluentParser Parser;
        private readonly Boolean Yolo;
        private readonly UInt32 repeatedMatcherLimit;

        static ValidExpressionGenerator ( )
        {
            AllChars = Enumerable.Range ( 0, Char.MaxValue ).Select ( i => ( Char ) i ).ToArray ( );
        }

        /// <summary>
        /// </summary>
        /// <param name="yolo">
        /// Whether to iterate through all chars from 0 to
        /// <see cref="Char.MaxValue" /> on <see cref="FilterFuncMatcher" />
        /// </param>
        public ValidExpressionGenerator ( FluentParser parser = null, Boolean yolo = true, UInt32 repeatedMatcherLimit = UInt32.MaxValue )
        {
            this.Parser = parser;
            this.Yolo = yolo;
            this.repeatedMatcherLimit = repeatedMatcherLimit;
        }

        private static HashSet<String> CartesianProduct ( IEnumerable<String> A, IEnumerable<String> B )
        {
            var set = new HashSet<String> ( );
            foreach ( var a in A )
                foreach ( var b in B )
                    set.Add ( a + b );
            return set;
        }

        public HashSet<String> Visit ( SequentialMatcher SequentialMatcher )
        {
            /*
             * 'a'{1, 2} 'a'{1, 5} 'a'{1, 2}
             * \_______/ \_______/ \_______/
             *     s₁        s₂        s₃
             * s₁ return:
             * ["a", "aa"]
             * s₂ return:
             * ["a", "aa", "aaa", "aaaa", "aaaaa"]
             * s₃ return:
             * ["a", "aa"]
             * s₁×s₂
             * ["aa", "aaa", "aaaa", "aaaaa", "aaaaaa", "aaaaaaa"]
             * (s₁×s₂)×s₃
             * ["aaa", "aaaa", "aaaaa", "aaaaaa", "aaaaaaa", "aaaaaaaa", "aaaaaaaaa"]
             */
            var resultSet = new HashSet<String> ( );
            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
            {
                HashSet<String> tmpRes = matcher.Accept ( this );
                // If result set is not empty, then do the
                // product, otherwise it's the same as multiplying
                // by 1
                resultSet = resultSet.Count > 0
                    ? CartesianProduct ( resultSet, tmpRes )
                    : new HashSet<String> ( tmpRes );
            }

            return resultSet;
        }

        public HashSet<String> Visit ( AlternatedMatcher AlternatedMatcher )
        {
            var set = new HashSet<String> ( );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
                set.UnionWith ( matcher.Accept ( this ) );
            return set;
        }

        public HashSet<String> Visit ( CharMatcher charMatcher )
            => new HashSet<String> ( new[] { charMatcher.StringFilter } );

        public HashSet<String> Visit ( RangeMatcher RangeMatcher )
        {
            var set = new HashSet<String> ( );
            for ( var i = ( Char ) RangeMatcher.Range.Start; i <= RangeMatcher.Range.End; i++ )
                set.Add ( i.ToString ( ) );
            return set;
        }

        public HashSet<String> Visit ( EOFMatcher eofMatcher ) => new HashSet<String> ( );

        public HashSet<String> Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            if ( !this.Yolo )
                throw new InvalidOperationException ( "Cannot find all possible inputs for a FilterFuncMatcher outside of #yolo mode." );
            var set = new HashSet<String> ( );
            for ( var i = '\0'; i < Char.MaxValue; i++ )
                if ( filterFuncMatcher.Filter ( i ) )
                    set.Add ( i.ToString ( ) );
            return set;
        }

        public HashSet<String> Visit ( IgnoreMatcher ignoreMatcher )
            => ignoreMatcher.PatternMatcher.Accept ( this );

        public HashSet<String> Visit ( JoinMatcher joinMatcher )
            => joinMatcher.PatternMatcher.Accept ( this );

        public HashSet<String> Visit ( MarkerMatcher markerMatcher )
            => markerMatcher.PatternMatcher.Accept ( this );

        public HashSet<String> Visit ( CharListMatcher CharListMatcher )
            => new HashSet<String> ( Array.ConvertAll ( CharListMatcher.Whitelist, ch => ch.ToString ( ) ) );

        public HashSet<String> Visit ( NegatedMatcher negatedMatcher )
        {
            if ( !( this.Yolo && negatedMatcher.PatternMatcher is AlternatedMatcher AlternatedMatcher
                && Array.TrueForAll ( AlternatedMatcher.PatternMatchers, m => m is CharMatcher || m is CharListMatcher
                    || m is RangeMatcher || m is FilterFuncMatcher ) ) )
                throw new InvalidOperationException ( "Cannot find all possible matches of NegatedMatcher whose inner matcher is a composition of CharMatchers, CharListMatchers, RangeMatchers or FilterFuncMatchers. It will also not work if the generator is outside of #yolo mode." );

            var set = new HashSet<String> ( );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
            {
                Char[] chars;
                if ( matcher is CharMatcher charMatcher )
                    chars = Array.FindAll ( AllChars, ch => ch != charMatcher.Filter );
                else if ( matcher is CharListMatcher CharListMatcher )
                    chars = Array.FindAll ( AllChars, ch => Array.IndexOf ( CharListMatcher.Whitelist, ch ) == -1 );
                else if ( matcher is RangeMatcher RangeMatcher )
                    chars = Array.FindAll ( AllChars, ch => !RangeMatcher.Range.ValueIn ( ch ) );
                else if ( matcher is FilterFuncMatcher filterFuncMatcher )
                    chars = Array.FindAll ( AllChars, ch => !filterFuncMatcher.Filter ( ch ) );
                else
                    throw new Exception ( "Somehow this passed." );
                set.UnionWith ( Array.ConvertAll ( chars, ch => ch.ToString ( ) ) );
            }

            return set;
        }

        public HashSet<String> Visit ( OptionalMatcher optionalMatcher )
        {
            HashSet<String> set = optionalMatcher.PatternMatcher.Accept ( this );
            set.Add ( "" );
            return set;
        }

        private static HashSet<String> CartesianPow ( IEnumerable<String> strings, UInt32 power )
        {
            if ( power == 0 )
                return new HashSet<String> ( new[] { "" } );
            else if ( power == 1 )
                return new HashSet<String> ( strings );

            HashSet<String> strs = CartesianProduct ( strings, strings );
            for ( var i = 3; i <= power; i++ )
                strs = CartesianProduct ( strs, strings );
            return strs;
        }

        public HashSet<String> Visit ( RepeatedMatcher repeatedMatcher )
        {
            if ( !this.Yolo && this.repeatedMatcherLimit > 100 )
                throw new InvalidOperationException ( "Cannot find all possible matches of a RepeatedMatcher whose End is greater than 100 outside of #yolo mode." );
            /*
             * s₁ = [...]
             * s₁{s, e} = Σs₁ⁿ where s ≤ n ≤ e
             */
            var resultSet = new HashSet<String> ( );
            HashSet<String> innerSet = repeatedMatcher.PatternMatcher.Accept ( this );
            for ( var i = repeatedMatcher.Range.Start; i <= Math.Min ( repeatedMatcher.Range.End, this.repeatedMatcherLimit ); i++ )
                resultSet.UnionWith ( CartesianPow ( innerSet, i ) );
            return resultSet;
        }

        public HashSet<String> Visit ( RulePlaceholder rulePlaceholder )
            => this.Parser?.RawRule ( rulePlaceholder.Name ).Accept ( this )
                ?? throw new InvalidOperationException ( "Cannot find a rule without a parser being provided" );

        public HashSet<String> Visit ( RuleWrapper ruleWrapper )
            => ruleWrapper.PatternMatcher.Accept ( this );

        public HashSet<String> Visit ( StringMatcher stringMatcher )
            => new HashSet<String> ( new[] { stringMatcher.StringFilter } );

        public HashSet<String> Visit ( SavingMatcher savingMatcher )
            => throw new NotImplementedException ( );

        public HashSet<String> Visit ( LoadingMatcher loadingMatcher )
            => throw new NotImplementedException ( );
    }
}
