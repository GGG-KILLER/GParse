using GParse.Verbose.Matchers;


namespace GParse.Verbose.Abstractions

{
    public interface IMatcherTreeVisitor<T>
    {
        T Visit ( SequentialMatcher SequentialMatcher );
        T Visit ( AlternatedMatcher AlternatedMatcher );
        T Visit ( CharMatcher charMatcher );
        T Visit ( RangeMatcher RangeMatcher );
        T Visit ( EOFMatcher eofMatcher );
        T Visit ( FilterFuncMatcher filterFuncMatcher );
        T Visit ( IgnoreMatcher ignoreMatcher );
        T Visit ( JoinMatcher joinMatcher );
        T Visit ( MarkerMatcher markerMatcher );
        T Visit ( CharListMatcher CharListMatcher );
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