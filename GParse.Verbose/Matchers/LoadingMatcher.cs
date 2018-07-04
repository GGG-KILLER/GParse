using System;
using System.Collections.Generic;
using System.Text;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
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
