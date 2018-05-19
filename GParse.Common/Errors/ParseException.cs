using System;

namespace GParse.Common.Errors
{
    public class ParseException : Exception
    {
        public readonly SourceLocation Location;
        public readonly String OriginalMessage;

        public ParseException ( SourceLocation location, String message ) : base ( $"{location.Line}:{location.Column}  {message}" )
        {
            this.Location = location;
            this.OriginalMessage = message;
        }

        public ParseException ( SourceLocation location, String message, Exception innerException ) : base ( $"{location.Line}:{location.Column}  {message}", innerException )
        {
            this.Location = location;
            this.OriginalMessage = message;
        }
    }
}
