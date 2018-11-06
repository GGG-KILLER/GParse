using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Fluent.Exceptions

{
    public class FatalParseException : ParsingException
    {
        public FatalParseException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public FatalParseException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
