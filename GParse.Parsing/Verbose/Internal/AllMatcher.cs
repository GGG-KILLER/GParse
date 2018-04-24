using System;
using System.Text;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct AllMatcher : IPatternMatcher
    {
        internal readonly IPatternMatcher[] PatternMatchers;

        public AllMatcher ( params IPatternMatcher[] patternMatchers )
        {
            if ( patternMatchers.Length < 1 )
                throw new ArgumentException ( "Must have at least 1 or more patterns to alternate.", nameof ( patternMatchers ) );
            this.PatternMatchers = patternMatchers;
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            foreach ( IPatternMatcher pm in this.PatternMatchers )
                if ( !pm.IsMatch ( reader ) )
                    return false;
            return true;
        }

        public String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;

            var sb = new StringBuilder ( );
            foreach ( IPatternMatcher pm in this.PatternMatchers )
                sb.Append ( pm.Match ( reader ) );
            return sb.ToString ( );
        }

        public void ResetInternalState ( )
        {
            foreach ( IPatternMatcher pm in this.PatternMatchers )
                pm.ResetInternalState ( );
        }
    }
}
