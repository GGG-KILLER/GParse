using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Matchers;

namespace GParse.Verbose
{
    public abstract class MatchTreewalker<T>
    {
        public virtual T Walk ( BaseMatcher baseMatcher )
        {
            if ( baseMatcher is AllMatcher allMatcher )
                return this.Walk ( allMatcher );
            else if ( baseMatcher is AnyMatcher anyMatcher )
                return this.Walk ( anyMatcher );
            else if ( baseMatcher is CharMatcher charMatcher )
                return this.Walk ( charMatcher );
            else if ( baseMatcher is CharRangeMatcher charRangeMatcher )
                return this.Walk ( charRangeMatcher );
            else if ( baseMatcher is FilterFuncMatcher filterFuncMatcher )
                return this.Walk ( filterFuncMatcher );
            else if ( baseMatcher is MultiCharMatcher multiCharMatcher )
                return this.Walk ( multiCharMatcher );
            else if ( baseMatcher is RulePlaceholder rulePlaceholder )
                return this.Walk ( rulePlaceholder );
            else if ( baseMatcher is StringMatcher stringMatcher )
                return this.Walk ( stringMatcher );
            else if ( baseMatcher is IgnoreMatcher ignoreMatcher )
                return this.Walk ( ignoreMatcher );
            else if ( baseMatcher is JoinMatcher joinMatcher )
                return this.Walk ( joinMatcher );
            else if ( baseMatcher is NegatedMatcher negatedMatcher )
                return this.Walk ( negatedMatcher );
            else if ( baseMatcher is OptionalMatcher optionalMatcher )
                return this.Walk ( optionalMatcher );
            else if ( baseMatcher is RepeatedMatcher repeatedMatcher )
                return this.Walk ( repeatedMatcher );
            else if ( baseMatcher is RuleWrapper ruleWrapper )
                return this.Walk ( ruleWrapper );
            throw new ArgumentException ( "Invalid matcher type." );
        }

        public abstract T Walk ( AllMatcher allMatcher );

        public abstract T Walk ( AnyMatcher anyMatcher );

        public abstract T Walk ( CharMatcher charMatcher );

        public abstract T Walk ( CharRangeMatcher charRangeMatcher );

        public abstract T Walk ( FilterFuncMatcher filterFuncMatcher );

        public abstract T Walk ( MultiCharMatcher multiCharMatcher );

        public abstract T Walk ( RulePlaceholder rulePlaceholder );

        public abstract T Walk ( StringMatcher stringMatcher );

        public abstract T Walk ( IgnoreMatcher ignoreMatcher );

        public abstract T Walk ( JoinMatcher joinMatcher );

        public abstract T Walk ( NegatedMatcher negatedMatcher );

        public abstract T Walk ( OptionalMatcher optionalMatcher );

        public abstract T Walk ( RepeatedMatcher repeatedMatcher );

        public abstract T Walk ( RuleWrapper ruleWrapper );
    }
}
