using System;

namespace GParse.Common.Errors
{
    public class ParseException : LocationBasedException
    {
        public ParseException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public ParseException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
