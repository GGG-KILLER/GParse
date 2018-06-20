using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public class EOFMatcher : BaseMatcher, IEquatable<EOFMatcher>
    {
        #region Generated Code

        public override Boolean Equals ( Object obj )
            => this.Equals ( obj as EOFMatcher );

        public Boolean Equals ( EOFMatcher other )
            => other != null;

        public override Int32 GetHashCode ( )
            => -79829576;

        public static Boolean operator == ( EOFMatcher matcher1, EOFMatcher matcher2 )
            => EqualityComparer<EOFMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( EOFMatcher matcher1, EOFMatcher matcher2 )
            => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
