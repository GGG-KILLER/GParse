using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Verbose.Exceptions

{
    public class RuleFailureException : ParseException
    {
        public readonly String RuleName;

        public RuleFailureException ( SourceLocation location, String ruleName ) : base ( location, $"Failed to match rule: {ruleName}" )
        {
            this.RuleName = ruleName;
        }

        public RuleFailureException ( SourceLocation location, String ruleName, Exception innerException ) : base ( location, $"Failed to match rule: {ruleName}", innerException )
        {
            this.RuleName = ruleName;
        }
    }
}
