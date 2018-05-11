using System;
using System.Text;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class AllMatcher : BaseMatcher
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

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader, out var _ ) )
                return null;

            var sb = new StringBuilder ( );
            foreach ( BaseMatcher pm in this.PatternMatchers )
                sb.Append ( pm.Match ( reader ) );
            return sb.ToString ( );
        }

        public override void ResetInternalState ( )
        {
            foreach ( BaseMatcher pm in this.PatternMatchers )
                pm.ResetInternalState ( );
        }
    }
}
