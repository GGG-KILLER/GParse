using System;
using System.Collections.Generic;
using System.Text;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public class SavingMatcher : MatcherWrapper
    {
        public readonly String SaveName;

        public SavingMatcher ( String saveName, BaseMatcher matcher ) : base ( matcher )
        {
            this.SaveName = saveName;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );
    }
}
