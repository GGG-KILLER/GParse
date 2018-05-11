using System;
using System.Linq.Expressions;
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

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            Boolean im = false;
            try
            {
                return im = this.PatternMatcher.IsMatch ( reader, offset );
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

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            throw new NotImplementedException ( );
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            throw new NotImplementedException ( );
        }
    }
}
