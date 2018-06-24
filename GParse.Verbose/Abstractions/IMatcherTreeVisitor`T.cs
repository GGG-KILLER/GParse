using GParse.Verbose.Matchers;

namespace GParse.Verbose.Abstractions
{
    public interface IMatcherTreeVisitor<T>
    {
        T Visit ( AllMatcher allMatcher );
        T Visit ( AnyMatcher anyMatcher );
        T Visit ( CharMatcher charMatcher );
        T Visit ( CharRangeMatcher charRangeMatcher );
        T Visit ( EOFMatcher eofMatcher );
        T Visit ( FilterFuncMatcher filterFuncMatcher );
        T Visit ( IgnoreMatcher ignoreMatcher );
        T Visit ( JoinMatcher joinMatcher );
        T Visit ( MarkerMatcher markerMatcher );
        T Visit ( MultiCharMatcher multiCharMatcher );
        T Visit ( NegatedMatcher negatedMatcher );
        T Visit ( OptionalMatcher optionalMatcher );
        T Visit ( RepeatedMatcher repeatedMatcher );
        T Visit ( RulePlaceholder rulePlaceholder );
        T Visit ( RuleWrapper ruleWrapper );
        T Visit ( StringMatcher stringMatcher );
    }
}