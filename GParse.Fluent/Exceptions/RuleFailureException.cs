using System;
using GParse;
using GParse.Errors;

namespace GParse.Fluent.Exceptions
{
    /// <summary>
    /// Thrown when a rule fails to execute
    /// </summary>
    public class RuleFailureException : FatalParsingException
    {
        /// <summary>
        /// The name of the rule that failed to execute
        /// </summary>
        public readonly String RuleName;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="ruleName"></param>
        public RuleFailureException ( SourceLocation location, String ruleName ) : base ( location, $"Failed to match rule: {ruleName}" )
        {
            this.RuleName = ruleName;
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="ruleName"></param>
        /// <param name="innerException"></param>
        public RuleFailureException ( SourceLocation location, String ruleName, Exception innerException ) : base ( location, $"Failed to match rule: {ruleName}", innerException )
        {
            this.RuleName = ruleName;
        }
    }
}
