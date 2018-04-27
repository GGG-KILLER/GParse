using System;
using GParse.Common;

namespace GParse.Verbose.Results
{
    internal class StringMatchResult : MatchResult
    {
        public readonly String Content;

        public StringMatchResult ( SourceRange range, String content, String name = null ) : base ( range, name )
        {
            this.Content = content;
        }
    }
}
