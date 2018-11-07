using GParse.Fluent.Matchers;

namespace GParse.Fluent.Abstractions

{
    public interface IMatcherTreeVisitor
    {
        void Visit ( SequentialMatcher sequentialMatcher );

        void Visit ( AlternatedMatcher alternatedMatcher );

        void Visit ( CharMatcher charMatcher );

        void Visit ( RangeMatcher RangeMatcher );

        void Visit ( EOFMatcher eofMatcher );

        void Visit ( FilterFuncMatcher filterFuncMatcher );

        void Visit ( IgnoreMatcher ignoreMatcher );

        void Visit ( JoinMatcher joinMatcher );

        void Visit ( MarkerMatcher markerMatcher );

        void Visit ( CharListMatcher charListMatcher );

        void Visit ( NegatedMatcher negatedMatcher );

        void Visit ( OptionalMatcher optionalMatcher );

        void Visit ( RepeatedMatcher repeatedMatcher );

        void Visit ( RulePlaceholder rulePlaceholder );

        void Visit ( RuleWrapper ruleWrapper );

        void Visit ( StringMatcher stringMatcher );

        void Visit ( SavingMatcher savingMatcher );

        void Visit ( LoadingMatcher loadingMatcher );
    }
}
