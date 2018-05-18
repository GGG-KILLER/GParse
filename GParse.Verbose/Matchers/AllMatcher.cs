using System;
using System.Collections.Generic;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class AllMatcher : BaseMatcher
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AllMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            length = offset;
            foreach ( BaseMatcher pm in this.PatternMatchers )
            {
                if ( !pm.IsMatch ( reader, out var sublen, length ) )
                {
                    length = 0;
                    return false;
                }
                length += sublen;
            }
            length -= offset;
            return true;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            try
            {
                var res = new List<String> ( );
                reader.Save ( );
                foreach ( BaseMatcher pm in this.PatternMatchers )
                    res.AddRange ( pm.Match ( reader ) );
                reader.DiscardSave ( );
                return res.ToArray ( );
            }
            catch ( ParseException )
            {
                reader.Load ( );
                throw;
            }
        }

        public override void ResetInternalState ( )
        {
            foreach ( BaseMatcher pm in this.PatternMatchers )
                pm.ResetInternalState ( );
        }
    }
}
