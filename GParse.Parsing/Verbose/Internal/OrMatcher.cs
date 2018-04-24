using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct AnyMatcher : IPatternMatcher
    {
        internal readonly IPatternMatcher[] PatternMatchers;

        public AnyMatcher ( params IPatternMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader ) )
                    return true;
            return false;
        }

        public String Match ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                if ( matcher.IsMatch ( reader ) )
                    return matcher.Match ( reader );
            return null;
        }

        public void ResetInternalState ( )
        {
            foreach ( IPatternMatcher matcher in this.PatternMatchers )
                matcher.ResetInternalState ( );
        }
    }
}
