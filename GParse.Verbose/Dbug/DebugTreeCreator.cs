using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public class DebugTreeCreator : MatcherTreeVisitor<DebugMatcher>
    {
        public override DebugMatcher Visit ( AllMatcher allMatcher )
            => new DebugMatcher ( new AllMatcher ( Array.ConvertAll ( allMatcher.PatternMatchers, this.Visit ) ) );

        public override DebugMatcher Visit ( AnyMatcher anyMatcher )
            => new DebugMatcher ( new AnyMatcher ( Array.ConvertAll ( anyMatcher.PatternMatchers, this.Visit ) ) );

        public override DebugMatcher Visit ( CharMatcher charMatcher )
            => new DebugMatcher ( charMatcher );

        public override DebugMatcher Visit ( CharRangeMatcher charRangeMatcher )
            => new DebugMatcher ( charRangeMatcher );

        public override DebugMatcher Visit ( FilterFuncMatcher filterFuncMatcher )
            => new DebugMatcher ( filterFuncMatcher );

        public override DebugMatcher Visit ( MultiCharMatcher multiCharMatcher )
            => new DebugMatcher ( multiCharMatcher );

        public override DebugMatcher Visit ( RulePlaceholder rulePlaceholder )
            => new DebugMatcher ( rulePlaceholder );

        public override DebugMatcher Visit ( StringMatcher stringMatcher )
            => new DebugMatcher ( stringMatcher );

        public override DebugMatcher Visit ( IgnoreMatcher ignoreMatcher )
            => new DebugMatcher ( new IgnoreMatcher ( this.Visit ( ignoreMatcher.PatternMatcher ) ) );

        public override DebugMatcher Visit ( JoinMatcher joinMatcher )
            => new DebugMatcher ( new JoinMatcher ( this.Visit ( joinMatcher.PatternMatcher ) ) );

        public override DebugMatcher Visit ( NegatedMatcher negatedMatcher )
            => new DebugMatcher ( new NegatedMatcher ( this.Visit ( negatedMatcher.PatternMatcher ) ) );

        public override DebugMatcher Visit ( OptionalMatcher optionalMatcher )
            => new DebugMatcher ( new OptionalMatcher ( this.Visit ( optionalMatcher.PatternMatcher ) ) );

        public override DebugMatcher Visit ( RepeatedMatcher repeatedMatcher )
            => new DebugMatcher ( new RepeatedMatcher ( this.Visit ( repeatedMatcher.PatternMatcher ),
                repeatedMatcher.Minimum, repeatedMatcher.Maximum ) );

        public override DebugMatcher Visit ( RuleWrapper ruleWrapper )
            => new DebugMatcher ( new RuleWrapper ( this.Visit ( ruleWrapper.PatternMatcher ), ruleWrapper.Name,
                ruleWrapper.RuleEnter, ruleWrapper.RuleMatched, ruleWrapper.RuleExit ) );

        public override DebugMatcher Visit ( MarkerMatcher markerMatcher )
            => new DebugMatcher ( new MarkerMatcher ( markerMatcher.Parser, this.Visit ( markerMatcher.PatternMatcher ) ) );

        public override DebugMatcher Visit ( EOFMatcher eofMatcher )
            => new DebugMatcher ( eofMatcher );
    }
}
