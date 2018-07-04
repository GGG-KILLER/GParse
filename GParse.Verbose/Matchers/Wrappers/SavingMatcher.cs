using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
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
