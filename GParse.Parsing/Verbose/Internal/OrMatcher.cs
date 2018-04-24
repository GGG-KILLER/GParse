using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal class AnyMatcher : BaseMatcher
    {
        internal readonly IPatternMatcher[] PatternMatchers;

        public AnyMatcher ( params IPatternMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader ) )
                    return true;
            return false;
        }

        public override String Match ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader ) )
                    return matcher.Match ( reader );
            return null;
        }

        public override void ResetInternalState ( )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                matcher.ResetInternalState ( );
        }
    }
}
