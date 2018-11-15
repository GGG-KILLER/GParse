using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors.MinimalExpression
{
    /// <summary>
    /// Generates expressions that hit all branches of a given parser language
    /// </summary>
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

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="parser"></param>
        public MinimalExpressionsGenerator ( FluentParser parser )
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Generates all expressions
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public String[] Generate ( BaseMatcher matcher ) => matcher.Accept ( this ).ToArray ( );

        #region IMatcherTreeVisitor<IEnumerable<String>>

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="sequentialMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( SequentialMatcher sequentialMatcher )
        {
            IEnumerable<String> last = sequentialMatcher.PatternMatchers[0].Accept ( this );
            for ( var i = 1; i < sequentialMatcher.PatternMatchers.Length; i++ )
                last = CartesianProduct ( last, sequentialMatcher.PatternMatchers[i].Accept ( this ) );
            return last;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="alternatedMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( AlternatedMatcher alternatedMatcher )
        {
            for ( var i = 0; i < alternatedMatcher.PatternMatchers.Length; i++ )
                foreach ( var elem in alternatedMatcher.PatternMatchers[i].Accept ( this ) )
                    yield return elem;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( CharMatcher charMatcher )
        {
            yield return charMatcher.StringFilter;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rangeMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( RangeMatcher rangeMatcher )
        {
            yield return rangeMatcher.Range.Start.ToString ( );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( EOFMatcher eofMatcher )
        {
            yield return "";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( IgnoreMatcher ignoreMatcher ) => ignoreMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( JoinMatcher joinMatcher ) => joinMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( MarkerMatcher markerMatcher ) => markerMatcher.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charListMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( CharListMatcher charListMatcher )
        {
            yield return charListMatcher.Whitelist[0].ToString ( );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( NegatedMatcher negatedMatcher ) => throw new NotSupportedException ( "Negated matchers are not supported." );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( OptionalMatcher optionalMatcher )
        {
            yield return "";
            foreach ( var option in optionalMatcher.PatternMatcher.Accept ( this ) )
                yield return option;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( RulePlaceholder rulePlaceholder ) => this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( RuleWrapper ruleWrapper ) => ruleWrapper.PatternMatcher.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( StringMatcher stringMatcher )
        {
            yield return stringMatcher.StringFilter;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( SavingMatcher savingMatcher ) => throw new NotSupportedException ( );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public IEnumerable<String> Visit ( LoadingMatcher loadingMatcher ) => throw new NotSupportedException ( );

        #endregion IMatcherTreeVisitor<IEnumerable<String>>
    }
}
