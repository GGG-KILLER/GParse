using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class InfiniteMatcher : MatcherWrapper
    {
        public InfiniteMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
            if ( matcher is OptionalMatcher )
                throw new InvalidOperationException ( "An infinite matcher of an optional matcher will cause an infinite loop." );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset )
        {
            length = offset;
            while ( base.IsMatch ( reader, out var tmpLength, length ) )
                length += tmpLength;
            length -= offset;
            return length != 0;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var res = new List<String> ( );
            SourceLocation loc = SourceLocation.Zero;
            try
            {
                while ( true )
                {
                    loc = reader.Location;
                    res.AddRange ( base.Match ( reader ) );
                }
            }
            catch ( ParseException )
            {
                // Revert to last location before erroring
                reader.Rewind ( loc );
                // Then find out if we re-throw or return
                if ( res.Count > 0 )
                    return res.ToArray ( );
                throw;
            }
        }
    }
}
