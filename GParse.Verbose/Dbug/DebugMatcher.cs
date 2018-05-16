using System;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    internal class DebugMatcher : BaseMatcher
    {
        public static Action<Object> LogLine = Console.WriteLine;

        internal BaseMatcher PatternMatcher;

        public DebugMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = 0;
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            LogLine?.Invoke ( $"{matcher}->Offset:     {offset}" );
            LogLine?.Invoke ( $"{matcher}->Expression: {reader} ({( Char ) reader.Peek ( offset )})" );
            var im = this.PatternMatcher.IsMatch ( reader, out length, offset );
            LogLine?.Invoke ( $"{matcher}->IsMatch:    {im}" );
            LogLine?.Invoke ( $"{matcher}->Length:     {length}" );
            return im;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            LogLine?.Invoke ( $"{matcher}->Expression: {reader}" );
            var m = this.PatternMatcher.Match ( reader );
            LogLine?.Invoke ( $"{matcher}->Match:      [{String.Join ( ", ", m )}]" );
            return m;
        }
    }
}
