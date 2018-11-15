using GParse.Fluent.Matchers;

namespace GParse.Fluent.Abstractions

{
    /// <summary>
    /// Defines the interface of a visitor that visits a tree of
    /// matchers and returns a value of type
    /// <typeparamref name="T" /> for each matcher
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatcherTreeVisitor<T>
    {
        /// <summary>
        /// Visits a <see cref="SequentialMatcher" />
        /// </summary>
        /// <param name="sequentialMatcher"></param>
        T Visit ( SequentialMatcher sequentialMatcher );

        /// <summary>
        /// Visits a <see cref="AlternatedMatcher" />
        /// </summary>
        /// <param name="alternatedMatcher"></param>
        T Visit ( AlternatedMatcher alternatedMatcher );

        /// <summary>
        /// Visits a <see cref="CharMatcher" />
        /// </summary>
        /// <param name="charMatcher"></param>
        T Visit ( CharMatcher charMatcher );

        /// <summary>
        /// Visits a <see cref="RangeMatcher" />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        T Visit ( RangeMatcher RangeMatcher );

        /// <summary>
        /// Visits a <see cref="EOFMatcher" />
        /// </summary>
        /// <param name="eofMatcher"></param>
        T Visit ( EOFMatcher eofMatcher );

        /// <summary>
        /// Visits a <see cref="FilterFuncMatcher" />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        T Visit ( FilterFuncMatcher filterFuncMatcher );

        /// <summary>
        /// Visits a <see cref="IgnoreMatcher" />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        T Visit ( IgnoreMatcher ignoreMatcher );

        /// <summary>
        /// Visits a <see cref="JoinMatcher" />
        /// </summary>
        /// <param name="joinMatcher"></param>
        T Visit ( JoinMatcher joinMatcher );

        /// <summary>
        /// Visits a <see cref="MarkerMatcher" />
        /// </summary>
        /// <param name="markerMatcher"></param>
        T Visit ( MarkerMatcher markerMatcher );

        /// <summary>
        /// Visits a <see cref="CharListMatcher" />
        /// </summary>
        /// <param name="charListMatcher"></param>
        T Visit ( CharListMatcher charListMatcher );

        /// <summary>
        /// Visits a <see cref="NegatedMatcher" />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        T Visit ( NegatedMatcher negatedMatcher );

        /// <summary>
        /// Visits a <see cref="OptionalMatcher" />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        T Visit ( OptionalMatcher optionalMatcher );

        /// <summary>
        /// Visits a <see cref="RepeatedMatcher" />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        T Visit ( RepeatedMatcher repeatedMatcher );

        /// <summary>
        /// Visits a <see cref="RulePlaceholder" />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        T Visit ( RulePlaceholder rulePlaceholder );

        /// <summary>
        /// Visits a <see cref="RuleWrapper" />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        T Visit ( RuleWrapper ruleWrapper );

        /// <summary>
        /// Visits a <see cref="StringMatcher" />
        /// </summary>
        /// <param name="stringMatcher"></param>
        T Visit ( StringMatcher stringMatcher );

        /// <summary>
        /// Visits a <see cref="SavingMatcher" />
        /// </summary>
        /// <param name="savingMatcher"></param>
        T Visit ( SavingMatcher savingMatcher );

        /// <summary>
        /// Visits a <see cref="LoadingMatcher" />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        T Visit ( LoadingMatcher loadingMatcher );
    }
}
