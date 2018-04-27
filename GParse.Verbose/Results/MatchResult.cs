using System;
using GParse.Common;

namespace GParse.Verbose.Results
{
    public abstract class MatchResult
    {
        public readonly String Name;
        public readonly SourceRange Range;

        protected MatchResult ( SourceRange range, String name )
        {
            this.Name = name;
            this.Range = range;
        }
    }
}
