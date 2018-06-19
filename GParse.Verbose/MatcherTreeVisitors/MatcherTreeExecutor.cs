using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.MatcherTreeVisitors
{
    public class MatcherTreeExecutor : MatcherTreeVisitor<String[]>
    {
        public override String[] Visit ( AllMatcher allMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( AnyMatcher anyMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( CharMatcher charMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( CharRangeMatcher charRangeMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( MultiCharMatcher multiCharMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( RulePlaceholder rulePlaceholder )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( StringMatcher stringMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( IgnoreMatcher ignoreMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( JoinMatcher joinMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( NegatedMatcher negatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( OptionalMatcher optionalMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( RepeatedMatcher repeatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( RuleWrapper ruleWrapper )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( MarkerMatcher markerMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override String[] Visit ( EOFMatcher eofMatcher )
        {
            throw new NotImplementedException ( );
        }
    }
}
