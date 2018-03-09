using System;

namespace GParse.Lexing.Errors
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
