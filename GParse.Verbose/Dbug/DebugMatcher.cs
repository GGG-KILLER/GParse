using System;
using System.Collections.Generic;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public class DebugMatcher : BaseMatcher, IEquatable<DebugMatcher>
    {
        internal readonly BaseMatcher PatternMatcher;

        public DebugMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
        {
            var matcher = MatcherDebug.GetMatcherName ( this.PatternMatcher );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->Offset:     {offset}" );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->Expression: {reader} ({( Char ) reader.Peek ( offset )})" );
            MatcherDebug.Logger.Indent ( $"{matcher}->IsMatch ( )" );
            var length  = this.PatternMatcher.MatchLength ( reader, offset );
            MatcherDebug.Logger.Outdent ( );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->IsMatch:    {length != -1}" );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->Length:     {length}" );
            return length;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var matcher = MatcherDebug.GetMatcherName ( this.PatternMatcher );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->Expression: {reader}" );
            MatcherDebug.Logger.Indent ( $"{matcher}->Match ( )" );
            var m       = this.PatternMatcher.Match ( reader );
            MatcherDebug.Logger.Outdent ( );
            MatcherDebug.Logger.WriteLine ( $"{matcher}->Match:      [{String.Join ( ", ", m )}]" );
            return m;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as DebugMatcher );
        }

        public Boolean Equals ( DebugMatcher other )
        {
            return other != null && EqualityComparer<BaseMatcher>.Default.Equals ( this.PatternMatcher, other.PatternMatcher );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1683483781;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher>.Default.GetHashCode ( this.PatternMatcher );
            return hashCode;
        }

        public static Boolean operator == ( DebugMatcher matcher1, DebugMatcher matcher2 ) => EqualityComparer<DebugMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( DebugMatcher matcher1, DebugMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
