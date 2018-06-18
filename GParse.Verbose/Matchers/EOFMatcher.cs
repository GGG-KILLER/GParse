using System;
using System.Collections.Generic;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public class EOFMatcher : BaseMatcher, IEquatable<EOFMatcher>
    {
        public override String[] Match ( SourceCodeReader reader )
            => this.MatchLength ( reader ) != -1
                ? Array.Empty<String> ( )
                : throw new ParseException ( reader.Location, "Expected EOF." );

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
            => reader.EOF ( ) ? 0 : -1;

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
    }
}
