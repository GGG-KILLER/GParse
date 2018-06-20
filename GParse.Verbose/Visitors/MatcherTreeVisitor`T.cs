using System;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors
{
    public abstract class MatcherTreeVisitor<T>
    {
        public virtual T Visit ( BaseMatcher baseMatcher )
        {
            if ( baseMatcher is AllMatcher allMatcher )
                return this.Visit ( allMatcher );
            else if ( baseMatcher is AnyMatcher anyMatcher )
                return this.Visit ( anyMatcher );
            else if ( baseMatcher is CharMatcher charMatcher )
                return this.Visit ( charMatcher );
            else if ( baseMatcher is CharRangeMatcher charRangeMatcher )
                return this.Visit ( charRangeMatcher );
            else if ( baseMatcher is FilterFuncMatcher filterFuncMatcher )
                return this.Visit ( filterFuncMatcher );
            else if ( baseMatcher is MultiCharMatcher multiCharMatcher )
                return this.Visit ( multiCharMatcher );
            else if ( baseMatcher is RulePlaceholder rulePlaceholder )
                return this.Visit ( rulePlaceholder );
            else if ( baseMatcher is StringMatcher stringMatcher )
                return this.Visit ( stringMatcher );
            else if ( baseMatcher is IgnoreMatcher ignoreMatcher )
                return this.Visit ( ignoreMatcher );
            else if ( baseMatcher is JoinMatcher joinMatcher )
                return this.Visit ( joinMatcher );
            else if ( baseMatcher is NegatedMatcher negatedMatcher )
                return this.Visit ( negatedMatcher );
            else if ( baseMatcher is OptionalMatcher optionalMatcher )
                return this.Visit ( optionalMatcher );
            else if ( baseMatcher is RepeatedMatcher repeatedMatcher )
                return this.Visit ( repeatedMatcher );
            else if ( baseMatcher is RuleWrapper ruleWrapper )
                return this.Visit ( ruleWrapper );
            else if ( baseMatcher is MarkerMatcher markerMatcher )
                return this.Visit ( markerMatcher );
            else if ( baseMatcher is EOFMatcher eofMatcher )
                return this.Visit ( eofMatcher );
            throw new ArgumentException ( $"Invalid matcher type: {baseMatcher.GetType ( ).FullName}" );
        }

        public abstract T Visit ( AllMatcher allMatcher );

        public abstract T Visit ( AnyMatcher anyMatcher );

        public abstract T Visit ( CharMatcher charMatcher );

        public abstract T Visit ( CharRangeMatcher charRangeMatcher );

        public abstract T Visit ( FilterFuncMatcher filterFuncMatcher );

        public abstract T Visit ( MultiCharMatcher multiCharMatcher );

        public abstract T Visit ( RulePlaceholder rulePlaceholder );

        public abstract T Visit ( StringMatcher stringMatcher );

        public abstract T Visit ( IgnoreMatcher ignoreMatcher );

        public abstract T Visit ( JoinMatcher joinMatcher );

        public abstract T Visit ( NegatedMatcher negatedMatcher );

        public abstract T Visit ( OptionalMatcher optionalMatcher );

        public abstract T Visit ( RepeatedMatcher repeatedMatcher );

        public abstract T Visit ( RuleWrapper ruleWrapper );

        public abstract T Visit ( MarkerMatcher markerMatcher );

        public abstract T Visit ( EOFMatcher eofMatcher );
    }
}
