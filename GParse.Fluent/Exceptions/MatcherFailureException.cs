using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Exceptions

{
    public class MatcherFailureException : ParsingException
    {
        public readonly BaseMatcher Matcher;

        public MatcherFailureException ( SourceLocation location, BaseMatcher matcher, String message ) : base ( location, message )
        {
            this.Matcher = matcher;
        }

        public MatcherFailureException ( SourceLocation location, BaseMatcher matcher, String message, Exception innerException ) : base ( location, message, innerException )
        {
            this.Matcher = matcher;
        }
    }
}
