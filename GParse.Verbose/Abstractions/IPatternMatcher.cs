using System;
using GParse.Common.IO;

namespace GParse.Verbose.Abstractions
{
    public interface IPatternMatcher
    {
        Boolean IsMatch ( SourceCodeReader reader );
        String Match ( SourceCodeReader reader );
        void ResetInternalState ( );
    }
}
