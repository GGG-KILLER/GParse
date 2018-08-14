using System;

namespace GParse.Common.Errors
{
    public class LexingException : LocationBasedException
    {
        public LexingException ( String message, SourceLocation location ) : base ( location, message )
        {
        }

        public LexingException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public LexingException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
