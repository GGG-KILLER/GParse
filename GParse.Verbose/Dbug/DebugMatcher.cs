using System;
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
            var im = false;
            try
            {
                return im = this.PatternMatcher.IsMatch ( reader, out length, offset );
            }
            finally
            {
                var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
                Console.WriteLine ( $"{matcher}->Offset:     {offset}" );
                Console.WriteLine ( $"{matcher}->Expression: {reader} ({( Char ) reader.Peek ( offset )})" );
                Console.WriteLine ( $"{matcher}->IsMatch:    {im}" );
            }
        }

        public override String Match ( SourceCodeReader reader )
        {
            String m = null;
            try
            {
                return m = this.PatternMatcher.Match ( reader );
            }
            finally
            {
                var matcher = MatcherDebug.GetMatcher ( this.PatternMatcher );
                Console.WriteLine ( $"{matcher}->Expression: {reader}" );
                Console.WriteLine ( $"{matcher}->Match:      {m}" );
            }
        }
    }
}
