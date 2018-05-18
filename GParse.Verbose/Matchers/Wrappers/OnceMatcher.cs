using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class OnceMatcher : MatcherWrapper
    {
        private Boolean Matched;

        public OnceMatcher ( BaseMatcher matcher ) : base ( matcher )
        {
            this.Matched = false;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 len, Int32 offset = 0 )
        {
            if ( this.Matched )
            {
                len = 0;
                return false;
            }
            return base.IsMatch ( reader, out len, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( !this.Matched )
            {
                this.Matched = true;
                return base.Match ( reader );
            }
            throw new ParseException ( reader.Location, $"Illegal pattern, the same pattern was already matched once." );
        }
    }
}
