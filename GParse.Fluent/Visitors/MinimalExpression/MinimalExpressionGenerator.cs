using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors.MinimalExpression
{
    public class MinimalExpressionsGenerator : IMatcherTreeVisitor<IEnumerable<StringBuilder>>
    {
        private readonly FluentParser Parser;

        #region Helper funcs

        private static IEnumerable<StringBuilder> CartesianProduct ( IEnumerable<StringBuilder> lhs, IEnumerable<StringBuilder> rhs )
        {
            foreach ( StringBuilder a in lhs )
                foreach ( StringBuilder b in rhs )
                    yield return new StringBuilder ( a.ToString ( ) + b );
        }

        #endregion Helper funcs

        public MinimalExpressionsGenerator ( FluentParser parser )
        {
            this.Parser = parser;
        }

        public String[] Generate ( BaseMatcher matcher ) =>
            matcher.Accept ( this ).Select ( builder => builder.ToString ( ) ).ToArray ( );

        #region IMatcherTreeVisitor<IEnumerable<StringBuilder>>

        public IEnumerable<StringBuilder> Visit ( SequentialMatcher sequentialMatcher )
        {
            IEnumerable<StringBuilder> last = sequentialMatcher.PatternMatchers[0].Accept ( this );
            for ( var i = 1; i < sequentialMatcher.PatternMatchers.Length; i++ )
                last = CartesianProduct ( last, sequentialMatcher.PatternMatchers[i].Accept ( this ) );
            return last;
        }

        public IEnumerable<StringBuilder> Visit ( AlternatedMatcher alternatedMatcher )
        {
            for ( var i = 0; i < alternatedMatcher.PatternMatchers.Length; i++ )
                foreach ( StringBuilder elem in alternatedMatcher.PatternMatchers[i].Accept ( this ) )
                    yield return elem;
        }

        public IEnumerable<StringBuilder> Visit ( CharMatcher charMatcher )
        {
            var list = new StringBuilder ( );
            list.Insert ( 0, charMatcher.StringFilter );
            yield return list;
        }

        public IEnumerable<StringBuilder> Visit ( RangeMatcher rangeMatcher )
        {
            Common.Math.Range<Char> range = rangeMatcher.Range;
            yield return new StringBuilder ( range.Start.ToString ( ) );
        }

        public IEnumerable<StringBuilder> Visit ( EOFMatcher eofMatcher )
        {
            yield return new StringBuilder ( );
        }

        public IEnumerable<StringBuilder> Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            var strs = new List<String> ( );
            for ( var i = Char.MinValue; i <= Char.MaxValue; i++ )
            {
                if ( filterFuncMatcher.Filter ( i ) )
                {
                    yield return new StringBuilder ( i.ToString ( ) );
                    yield break;
                }
            }

            throw new InvalidOperationException ( $"Cannot get a minimal expression for a predicate which doesn't accepts any chars." );
        }

        public IEnumerable<StringBuilder> Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<StringBuilder> Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<StringBuilder> Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<StringBuilder> Visit ( CharListMatcher charListMatcher )
        {
            yield return new StringBuilder ( charListMatcher.Whitelist[0].ToString ( ) );
        }

        public IEnumerable<StringBuilder> Visit ( NegatedMatcher negatedMatcher ) => throw new NotSupportedException ( "Negated matchers are not supported." );

        public IEnumerable<StringBuilder> Visit ( OptionalMatcher optionalMatcher )
        {
            yield return new StringBuilder ( );
            foreach ( StringBuilder option in optionalMatcher.PatternMatcher.Accept ( this ) )
                yield return option;
        }

        public IEnumerable<StringBuilder> Visit ( RepeatedMatcher repeatedMatcher )
        {
            // If we can, don't insert anything
            if ( repeatedMatcher.Range.Start == 0 )
            {
                yield return new StringBuilder ( );
                yield break;
            }

            IEnumerable<StringBuilder> innerMatcherExpressions = repeatedMatcher.PatternMatcher.Accept ( this );
            IEnumerable<StringBuilder> expressions = innerMatcherExpressions;
            for ( var i = 2; i <= repeatedMatcher.Range.Start; i++ )
                expressions = CartesianProduct ( expressions, innerMatcherExpressions );

            foreach ( StringBuilder expression in expressions )
                yield return expression;
        }

        public IEnumerable<StringBuilder> Visit ( RulePlaceholder rulePlaceholder ) => this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );

        public IEnumerable<StringBuilder> Visit ( RuleWrapper ruleWrapper ) => ruleWrapper.PatternMatcher.Accept ( this );

        public IEnumerable<StringBuilder> Visit ( StringMatcher stringMatcher )
        {
            yield return new StringBuilder ( stringMatcher.StringFilter );
        }

        public IEnumerable<StringBuilder> Visit ( SavingMatcher savingMatcher ) => throw new NotSupportedException ( );

        public IEnumerable<StringBuilder> Visit ( LoadingMatcher loadingMatcher ) => throw new NotSupportedException ( );

        #endregion IMatcherTreeVisitor<IEnumerable<StringBuilder>>
    }
}
