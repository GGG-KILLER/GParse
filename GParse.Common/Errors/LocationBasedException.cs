using System;

namespace GParse.Common.Errors
{
    /// <summary>
    /// Defines the base exception for a location-based exception
    /// </summary>
    public abstract class LocationBasedException : Exception
    {
        /// <summary>
        /// The location where the exception happened
        /// </summary>
        public readonly SourceLocation Location;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        protected LocationBasedException ( SourceLocation location, String message ) : base ( message )
        {
            this.Location = location;
        }

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        protected LocationBasedException ( SourceLocation location, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Location = location;
        }
    }
}
