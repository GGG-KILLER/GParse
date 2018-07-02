using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Verbose.Parsing.Matchers;


namespace GParse.Verbose.Parsing.Exceptions

{
    public class MatcherFailureException : ParseException
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
