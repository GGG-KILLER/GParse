using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class RuleWrapper : BaseMatcher
    {
        internal readonly String Name;
        internal readonly BaseMatcher PatternMatcher;
        internal readonly Action<String, String> RuleMatched;
        internal readonly Action<String> RuleExit;
        internal readonly Action<String> RuleEnter;

        public RuleWrapper ( BaseMatcher Matcher, String Name, Action<String> RuleEnter, Action<String, String> RuleMatched, Action<String> RuleExit )
        {
            this.Name = Name;
            this.PatternMatcher = Matcher;
            // We need these as delegates because when
            // everything's compiled into expressions we won't
            // have instances to call events on. (future proofing™)
            this.RuleEnter = RuleEnter;
            this.RuleExit = RuleExit;
            this.RuleMatched = RuleMatched;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            return this.PatternMatcher.IsMatch ( reader, out length, offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            this.RuleEnter ( this.Name );
            var match = this.PatternMatcher.Match ( reader );
            this.RuleMatched ( this.Name, match );
            this.RuleExit ( this.Name );
            return match;
        }
    }
}
