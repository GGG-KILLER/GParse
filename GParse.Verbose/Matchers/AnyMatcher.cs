using System;
using System.Diagnostics;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Exceptions;

namespace GParse.Verbose.Matchers
{
    public sealed class AnyMatcher : BaseMatcher
    {
        internal readonly BaseMatcher[] PatternMatchers;

        public AnyMatcher ( params BaseMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            foreach ( BaseMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader, out length, offset ) )
                    return true;
            length = 0;
            return false;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            foreach ( BaseMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader, out var _ ) )
                    return matcher.Match ( reader );
            throw new ParseException ( reader.Location, "Failed to match any of the provided patterns." );
        }

        public override void ResetInternalState ( )
        {
            foreach ( BaseMatcher matcher in this.PatternMatchers )
                matcher.ResetInternalState ( );
        }
    }
}
