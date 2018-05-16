using System;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    internal class DebugMatcher : BaseMatcher
    {
        public static Action<Object> LogLine = Console.WriteLine;
        private static Int32 depth;

        internal BaseMatcher PatternMatcher;

        public DebugMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = 0;
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            var indent = new String ( '\t', depth );
            LogLine?.Invoke ( $"{indent}{matcher}->Offset:     {offset}" );
            LogLine?.Invoke ( $"{indent}{matcher}->Expression: {reader} ({( Char ) reader.Peek ( offset )})" );
            depth++;
            var im = this.PatternMatcher.IsMatch ( reader, out length, offset );
            depth--;
            LogLine?.Invoke ( $"{indent}{matcher}->IsMatch:    {im}" );
            LogLine?.Invoke ( $"{indent}{matcher}->Length:     {length}" );
            return im;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
            var indent = new String ( '\t', depth );
            LogLine?.Invoke ( $"{indent}{matcher}->Expression: {reader}" );
            depth++;
            var m = this.PatternMatcher.Match ( reader );
            depth--;
            LogLine?.Invoke ( $"{indent}{matcher}->Match:      [{String.Join ( ", ", m )}]" );
            return m;
        }
    }
}
