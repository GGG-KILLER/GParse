using System;

namespace GParse.Common.Errors
{
    public abstract class LocationBasedException : Exception
    {
        public readonly SourceLocation Location;

        protected LocationBasedException ( SourceLocation location, String message ) : base ( message )
        {
            this.Location = location;
        }

        protected LocationBasedException ( SourceLocation location, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Location = location;
        }
    }
}
