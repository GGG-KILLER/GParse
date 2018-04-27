using System;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    internal class AliasedMatcher : BaseMatcher
    {
        internal readonly String Name;
        internal readonly IPatternMatcher PatternMatcher;

        public AliasedMatcher ( IPatternMatcher matcher, String Name )
        {
            this.Name = Name;
            this.PatternMatcher = matcher;
        }

        public override Boolean IsMatch ( SourceCodeReader reader )
        {
            return this.PatternMatcher.IsMatch ( reader );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;

            var match = this.PatternMatcher.Match ( reader );
            this.OnMatch ( this.Name, match );
            return match;
        }

        public override void ResetInternalState ( )
        {
            // noop
        }
    }
}
