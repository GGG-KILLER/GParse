using GParse.Fluent.Matchers;


namespace GParse.Fluent.Abstractions

{
    public interface IMatcherTreeVisitor<T>
    {
        T Visit ( SequentialMatcher sequentialMatcher );
        T Visit ( AlternatedMatcher alternatedMatcher );
        T Visit ( CharMatcher charMatcher );
        T Visit ( RangeMatcher RangeMatcher );
        T Visit ( EOFMatcher eofMatcher );
        T Visit ( FilterFuncMatcher filterFuncMatcher );
        T Visit ( IgnoreMatcher ignoreMatcher );
        T Visit ( JoinMatcher joinMatcher );
        T Visit ( MarkerMatcher markerMatcher );
        T Visit ( CharListMatcher charListMatcher );
        T Visit ( NegatedMatcher negatedMatcher );
        T Visit ( OptionalMatcher optionalMatcher );
        T Visit ( RepeatedMatcher repeatedMatcher );
        T Visit ( RulePlaceholder rulePlaceholder );
        T Visit ( RuleWrapper ruleWrapper );
        T Visit ( StringMatcher stringMatcher );
        T Visit ( SavingMatcher savingMatcher );
        T Visit ( LoadingMatcher loadingMatcher );
    }
}