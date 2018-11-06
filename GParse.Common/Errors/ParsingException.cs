using System;

namespace GParse.Common.Errors
{
    public class ParsingException : LocationBasedException
    {
        public ParsingException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public ParsingException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
