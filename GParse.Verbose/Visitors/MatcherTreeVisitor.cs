using System;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Visitors
{
    public abstract class MatcherTreeVisitor
    {
        public virtual void Visit ( BaseMatcher baseMatcher )
        {
            if ( baseMatcher is AllMatcher allMatcher )
                this.Visit ( allMatcher );
            else if ( baseMatcher is AnyMatcher anyMatcher )
                this.Visit ( anyMatcher );
            else if ( baseMatcher is CharMatcher charMatcher )
                this.Visit ( charMatcher );
            else if ( baseMatcher is CharRangeMatcher charRangeMatcher )
                this.Visit ( charRangeMatcher );
            else if ( baseMatcher is FilterFuncMatcher filterFuncMatcher )
                this.Visit ( filterFuncMatcher );
            else if ( baseMatcher is MultiCharMatcher multiCharMatcher )
                this.Visit ( multiCharMatcher );
            else if ( baseMatcher is RulePlaceholder rulePlaceholder )
                this.Visit ( rulePlaceholder );
            else if ( baseMatcher is StringMatcher stringMatcher )
                this.Visit ( stringMatcher );
            else if ( baseMatcher is IgnoreMatcher ignoreMatcher )
                this.Visit ( ignoreMatcher );
            else if ( baseMatcher is JoinMatcher joinMatcher )
                this.Visit ( joinMatcher );
            else if ( baseMatcher is NegatedMatcher negatedMatcher )
                this.Visit ( negatedMatcher );
            else if ( baseMatcher is OptionalMatcher optionalMatcher )
                this.Visit ( optionalMatcher );
            else if ( baseMatcher is RepeatedMatcher repeatedMatcher )
                this.Visit ( repeatedMatcher );
            else if ( baseMatcher is RuleWrapper ruleWrapper )
                this.Visit ( ruleWrapper );
            else if ( baseMatcher is MarkerMatcher markerMatcher )
                this.Visit ( markerMatcher );
            else if ( baseMatcher is EOFMatcher eofMatcher )
                this.Visit ( eofMatcher );
            else
                throw new ArgumentException ( $"Invalid matcher type: {baseMatcher.GetType ( ).FullName}" );
        }

        public abstract void Visit ( AllMatcher allMatcher );

        public abstract void Visit ( AnyMatcher anyMatcher );

        public abstract void Visit ( CharMatcher charMatcher );

        public abstract void Visit ( CharRangeMatcher charRangeMatcher );

        public abstract void Visit ( FilterFuncMatcher filterFuncMatcher );

        public abstract void Visit ( MultiCharMatcher multiCharMatcher );

        public abstract void Visit ( RulePlaceholder rulePlaceholder );

        public abstract void Visit ( StringMatcher stringMatcher );

        public abstract void Visit ( IgnoreMatcher ignoreMatcher );

        public abstract void Visit ( JoinMatcher joinMatcher );

        public abstract void Visit ( NegatedMatcher negatedMatcher );

        public abstract void Visit ( OptionalMatcher optionalMatcher );

        public abstract void Visit ( RepeatedMatcher repeatedMatcher );

        public abstract void Visit ( RuleWrapper ruleWrapper );

        public abstract void Visit ( MarkerMatcher markerMatcher );

        public abstract void Visit ( EOFMatcher eofMatcher );
    }
}
