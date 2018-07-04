using System;

namespace GParse.Common.Errors
{
    public class LexException : LocationBasedException
    {
        public LexException ( String message, SourceLocation location ) : base ( location, message )
        {
        }

        public LexException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public LexException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
