using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Verbose.Parsing.Exceptions

{
    public class FatalParseException : ParseException
    {
        public FatalParseException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public FatalParseException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
