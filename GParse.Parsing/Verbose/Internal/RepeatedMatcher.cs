using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct RepeatedMatcher : IPatternMatcher
    {
        internal readonly IPatternMatcher Matcher;
        internal readonly Int32 Limit;
        internal Int32 Matches;

        public RepeatedMatcher ( IPatternMatcher matcher, Int32 limit )
        {
            this.Matcher = matcher;
            this.Limit = limit;
            this.Matches = 0;
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            return !reader.EOF ( ) && ( this.Limit == -1 || this.Matches < this.Limit ) && this.Matcher.IsMatch ( reader );
        }

        public String Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader ) )
            {
                this.Matches++;
                return this.Matcher.Match ( reader );
            }
            return null;
        }

        public void ResetInternalState ( )
        {
            this.Matches = 0;
        }
    }
}
