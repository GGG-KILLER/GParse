using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class NegatedMatcher : MatcherWrapper
    {
        public NegatedMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = 0;
            return !base.IsMatch ( reader, out var _, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            try
            {
                reader.Save ( );
                base.Match ( reader );
            }
            catch ( ParseException )
            {
                // It failed as expected. Discard the save and
                // move on
                reader.DiscardSave ( );
                return Array.Empty<String> ( );
            }
            // Oshit waddup, this shouldn't match but did, so we
            // need to revert the changes
            reader.Load ( );
            throw new ParseException ( reader.Location, $"Matched expression that wasn't meant to be matched." );
        }
    }
}
