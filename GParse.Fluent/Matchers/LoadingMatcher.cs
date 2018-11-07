using System;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public class LoadingMatcher : BaseMatcher
    {
        public readonly String SaveName;

        public LoadingMatcher ( String saveName )
        {
            this.SaveName = saveName;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );
    }
}
