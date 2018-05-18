using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class RepeatedMatcher : MatcherWrapper
    {
        internal readonly Int32 Limit;

        public RepeatedMatcher ( BaseMatcher matcher, Int32 limit ) : base ( matcher )
        {
            this.Limit = limit;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = 0;
            while ( base.IsMatch ( reader, out var subLength, length ) )
                length += subLength;
            return length != 0;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            var res = new List<String> ( );
            SourceLocation loc = SourceLocation.Max;
            try
            {
                for ( var i = 0; i < this.Limit; i++ )
                {
                    loc = reader.Location;
                    res.AddRange ( base.Match ( reader ) );
                }
            }
            catch ( ParseException )
            {
                reader.Rewind ( loc );
                if ( res.Count == 0 )
                    throw;
            }
            return res.ToArray ( );
        }
    }
}
