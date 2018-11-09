using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors.MinimalExpression
{
    public class MinimalExpressionsGenerator : IMatcherTreeVisitor<IEnumerable<String>>
    {
        private readonly FluentParser Parser;

        #region Helper funcs

        private static IEnumerable<String> CartesianProduct ( IEnumerable<String> lhs, IEnumerable<String> rhs )
        {
            foreach ( var a in lhs )
                foreach ( var b in rhs )
                    yield return a + b;
        }

        #endregion Helper funcs

        public MinimalExpressionsGenerator ( FluentParser parser )
        {
            this.Parser = parser;
        }

        public String[] Generate ( BaseMatcher matcher ) => matcher.Accept ( this ).ToArray ( );

        #region IMatcherTreeVisitor<IEnumerable<String>>

        public IEnumerable<String> Visit ( SequentialMatcher sequentialMatcher )
        {
            IEnumerable<String> last = sequentialMatcher.PatternMatchers[0].Accept ( this );
            for ( var i = 1; i < sequentialMatcher.PatternMatchers.Length; i++ )
                last = CartesianProduct ( last, sequentialMatcher.PatternMatchers[i].Accept ( this ) );
            return last;
        }

        public IEnumerable<String> Visit ( AlternatedMatcher alternatedMatcher )
        {
            for ( var i = 0; i < alternatedMatcher.PatternMatchers.Length; i++ )
                foreach ( var elem in alternatedMatcher.PatternMatchers[i].Accept ( this ) )
                    yield return elem;
        }

        public IEnumerable<String> Visit ( CharMatcher charMatcher )
        {
            yield return charMatcher.StringFilter;
        }

        public IEnumerable<String> Visit ( RangeMatcher rangeMatcher )
        {
            yield return rangeMatcher.Range.Start.ToString ( );
        }

        public IEnumerable<String> Visit ( EOFMatcher eofMatcher )
        {
            yield return "";
        }

        public IEnumerable<String> Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            for ( var i = Char.MinValue; i <= Char.MaxValue; i++ )
            {
                if ( filterFuncMatcher.Filter ( i ) )
                {
                    yield return i.ToString ( );
                    yield break;
                }
            }

            throw new InvalidOperationException ( $"Cannot get a minimal expression for a predicate which doesn't accepts any chars." );
        }

        public IEnumerable<String> Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<String> Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<String> Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        public IEnumerable<String> Visit ( CharListMatcher charListMatcher )
        {
            yield return charListMatcher.Whitelist[0].ToString ( );
        }

        public IEnumerable<String> Visit ( NegatedMatcher negatedMatcher ) => throw new NotSupportedException ( "Negated matchers are not supported." );

        public IEnumerable<String> Visit ( OptionalMatcher optionalMatcher )
        {
            yield return "";
            foreach ( var option in optionalMatcher.PatternMatcher.Accept ( this ) )
                yield return option;
        }

        public IEnumerable<String> Visit ( RepeatedMatcher repeatedMatcher )
        {
            // If we can, don't insert anything
            if ( repeatedMatcher.Range.Start == 0 )
            {
                yield return "";
                yield break;
            }

            IEnumerable<String> innerMatcherExpressions = repeatedMatcher.PatternMatcher.Accept ( this );
            IEnumerable<String> expressions = innerMatcherExpressions;
            for ( var i = 2; i <= repeatedMatcher.Range.Start; i++ )
                expressions = CartesianProduct ( expressions, innerMatcherExpressions );

            foreach ( var expression in expressions )
                yield return expression;
        }

        public IEnumerable<String> Visit ( RulePlaceholder rulePlaceholder ) => this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );

        public IEnumerable<String> Visit ( RuleWrapper ruleWrapper ) => ruleWrapper.PatternMatcher.Accept ( this );

        public IEnumerable<String> Visit ( StringMatcher stringMatcher )
        {
            yield return stringMatcher.StringFilter;
        }

        public IEnumerable<String> Visit ( SavingMatcher savingMatcher ) => throw new NotSupportedException ( );

        public IEnumerable<String> Visit ( LoadingMatcher loadingMatcher ) => throw new NotSupportedException ( );

        #endregion IMatcherTreeVisitor<IEnumerable<String>>
    }
}
