using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class OptionalMatcher : MatcherWrapper
    {
        public OptionalMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            if ( !base.IsMatch ( reader, out length, offset ) )
                length = 0;
            return true;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            try
            {
                return base.Match ( reader );
            }
            catch ( ParseException )
            {
                return Array.Empty<String> ( );
            }
        }
    }
}
