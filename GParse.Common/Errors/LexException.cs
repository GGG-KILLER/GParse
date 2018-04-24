using System;

namespace GParse.Common.Errors
{
    public class LexException : Exception
    {
        public readonly SourceLocation Location;

        public LexException ( String message, SourceLocation Location ) : base ( message )
        {
            this.Location = Location;
        }
    }
}
