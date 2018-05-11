using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Abstractions
{
    public interface IPatternMatcher
    {
        Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 );
        String Match ( SourceCodeReader reader );
        void ResetInternalState ( );
    }
}
