using GParse.Verbose.Matchers;


namespace GParse.Verbose.Abstractions

{
    public interface IMatcherTreeVisitor
    {
        void Visit ( SequentialMatcher SequentialMatcher );
        void Visit ( AlternatedMatcher AlternatedMatcher );
        void Visit ( CharMatcher charMatcher );
        void Visit ( RangeMatcher RangeMatcher );
        void Visit ( EOFMatcher eofMatcher );
        void Visit ( FilterFuncMatcher filterFuncMatcher );
        void Visit ( IgnoreMatcher ignoreMatcher );
        void Visit ( JoinMatcher joinMatcher );
        void Visit ( MarkerMatcher markerMatcher );
        void Visit ( CharListMatcher CharListMatcher );
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