using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Exceptions
{
    /// <summary>
    /// Thrown when a matcher fails to match
    /// </summary>
    public class MatcherFailureException : FatalParsingException
    {
        /// <summary>
        /// The matcher that failed to match
        /// </summary>
        public readonly BaseMatcher Matcher;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matcher"></param>
        /// <param name="message"></param>
        public MatcherFailureException ( SourceLocation location, BaseMatcher matcher, String message ) : base ( location, message )
        {
            this.Matcher = matcher;
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matcher"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MatcherFailureException ( SourceLocation location, BaseMatcher matcher, String message, Exception innerException ) : base ( location, message, innerException )
        {
            this.Matcher = matcher;
        }
    }
}
