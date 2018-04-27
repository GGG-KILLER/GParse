using System;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class RepeatedMatcher : BaseMatcher
    {
        internal readonly IPatternMatcher PatternMatcher;
        internal readonly Int32 Limit;

        public RepeatedMatcher ( IPatternMatcher matcher, Int32 limit )
        {
            this.PatternMatcher = matcher;
            this.Limit = limit;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && this.PatternMatcher.IsMatch ( reader );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader ) )
            {
                var sb = new StringBuilder ( );
                for ( var i = 0; i < this.Limit && this.IsMatch ( reader ); i++ )
                    sb.Append ( this.PatternMatcher.Match ( reader ) );
                return sb.ToString ( );
            }
            return null;
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
