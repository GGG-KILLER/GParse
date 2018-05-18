using System;
using System.Diagnostics;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    internal class DebugMatcher : BaseMatcher
    {
        internal BaseMatcher PatternMatcher;

        public DebugMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            Debug.WriteLine ( $"{matcher}->Offset:     {offset}" );
            Debug.WriteLine ( $"{matcher}->Expression: {reader} ({( Char ) reader.Peek ( offset )})" );
            Debug.Indent ( );
            var im = this.PatternMatcher.IsMatch ( reader, out length, offset );
            Debug.Unindent ( );
            Debug.WriteLine ( $"{matcher}->IsMatch:    {im}" );
            Debug.WriteLine ( $"{matcher}->Length:     {length}" );
            return im;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            Debug.WriteLine ( $"{matcher}->Expression: {reader}" );
            Debug.Indent ( );
            var m = this.PatternMatcher.Match ( reader );
            Debug.Unindent ( );
            Debug.WriteLine ( $"{matcher}->Match:      [{String.Join ( ", ", m )}]" );
            return m;
        }
    }
}
