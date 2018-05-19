using System;
using System.Diagnostics;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class RuleWrapper : MatcherWrapper
    {
        internal readonly String Name;
        internal readonly Action<String, String[]> RuleMatched;
        internal readonly Action<String> RuleExit;
        internal readonly Action<String> RuleEnter;

        public RuleWrapper ( BaseMatcher Matcher, String Name, Action<String> RuleEnter, Action<String, String[]> RuleMatched, Action<String> RuleExit )
            : base ( Matcher )
        {
            this.Name = Name;
            // We need these as delegates because when
            // everything's compiled into expression trees we
            // won't have instances to call events on. (future
            // proofing™ (actually I tried to do it once but
            // failed, but shhhh))
            this.RuleEnter = RuleEnter;
            this.RuleExit = RuleExit;
            this.RuleMatched = RuleMatched;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            return base.IsMatch ( reader, out length, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            try
            {
                this.RuleEnter ( this.Name );
                this.RuleMatched ( this.Name, base.Match ( reader ) );
                return Array.Empty<String> ( );
            }
            finally
            {
                this.RuleExit ( this.Name );
            }
        }
    }
}
