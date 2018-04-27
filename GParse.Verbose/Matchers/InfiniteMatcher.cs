using System;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class InfiniteMatcher : BaseMatcher
    {
        internal readonly IPatternMatcher PatternMatcher;

        public InfiniteMatcher ( IPatternMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
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
                while ( this.IsMatch ( reader ) )
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
