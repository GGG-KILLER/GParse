using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Verbose.Exceptions
{
    public class MultiErrorParseException : ParseException
    {
        public readonly ParseException[] Errors;

        public MultiErrorParseException ( SourceLocation location, ParseException[] errors ) : base ( location, "Multiple errors." )
        {
            this.Errors = errors;
        }
    }
}
