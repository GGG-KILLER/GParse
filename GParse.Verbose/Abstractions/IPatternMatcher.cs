using System;
using GParse.Common.IO;

namespace GParse.Verbose.Abstractions
{
    public interface IPatternMatcher
    {
        Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 );

        String[] Match ( SourceCodeReader reader );
    }
}
