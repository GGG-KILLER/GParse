using GParse.Fluent.Matchers;

namespace GParse.Fluent.Abstractions
{
    /// <summary>
    /// Defines the interface of a visitor of a tree of matchers
    /// </summary>
    public interface IMatcherTreeVisitor
    {
        /// <summary>
        /// Visits a <see cref="SequentialMatcher" />
        /// </summary>
        /// <param name="sequentialMatcher"></param>
        void Visit ( SequentialMatcher sequentialMatcher );

        /// <summary>
        /// Visits a <see cref="AlternatedMatcher" />
        /// </summary>
        /// <param name="alternatedMatcher"></param>
        void Visit ( AlternatedMatcher alternatedMatcher );

        /// <summary>
        /// Visits a <see cref="CharMatcher" />
        /// </summary>
        /// <param name="charMatcher"></param>
        void Visit ( CharMatcher charMatcher );

        /// <summary>
        /// Visits a <see cref="RangeMatcher" />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        void Visit ( RangeMatcher RangeMatcher );

        /// <summary>
        /// Visits a <see cref="EOFMatcher" />
        /// </summary>
        /// <param name="eofMatcher"></param>
        void Visit ( EOFMatcher eofMatcher );

        /// <summary>
        /// Visits a <see cref="FilterFuncMatcher" />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        void Visit ( FilterFuncMatcher filterFuncMatcher );

        /// <summary>
        /// Visits a <see cref="IgnoreMatcher" />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        void Visit ( IgnoreMatcher ignoreMatcher );

        /// <summary>
        /// Visits a <see cref="JoinMatcher" />
        /// </summary>
        /// <param name="joinMatcher"></param>
        void Visit ( JoinMatcher joinMatcher );

        /// <summary>
        /// Visits a <see cref="MarkerMatcher" />
        /// </summary>
        /// <param name="markerMatcher"></param>
        void Visit ( MarkerMatcher markerMatcher );

        /// <summary>
        /// Visits a <see cref="CharListMatcher" />
        /// </summary>
        /// <param name="charListMatcher"></param>
        void Visit ( CharListMatcher charListMatcher );

        /// <summary>
        /// Visits a <see cref="NegatedMatcher" />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        void Visit ( NegatedMatcher negatedMatcher );

        /// <summary>
        /// Visits a <see cref="OptionalMatcher" />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        void Visit ( OptionalMatcher optionalMatcher );

        /// <summary>
        /// Visits a <see cref="RepeatedMatcher" />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        void Visit ( RepeatedMatcher repeatedMatcher );

        /// <summary>
        /// Visits a <see cref="RulePlaceholder" />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        void Visit ( RulePlaceholder rulePlaceholder );

        /// <summary>
        /// Visits a <see cref="RuleWrapper" />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        void Visit ( RuleWrapper ruleWrapper );

        /// <summary>
        /// Visits a <see cref="StringMatcher" />
        /// </summary>
        /// <param name="stringMatcher"></param>
        void Visit ( StringMatcher stringMatcher );

        /// <summary>
        /// Visits a <see cref="SavingMatcher" />
        /// </summary>
        /// <param name="savingMatcher"></param>
        void Visit ( SavingMatcher savingMatcher );

        /// <summary>
        /// Visits a <see cref="LoadingMatcher" />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        void Visit ( LoadingMatcher loadingMatcher );
    }
}
