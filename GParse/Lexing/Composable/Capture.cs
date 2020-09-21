using System;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a capture.
    /// </summary>
    public readonly struct Capture
    {
        /// <summary>
        /// The starting position of the capture.
        /// </summary>
        public readonly Int32 Start;

        /// <summary>
        /// The length of the capture.
        /// </summary>
        public readonly Int32 Length;

        /// <summary>
        /// Initializes a new capture.
        /// </summary>
        /// <param name="start"><inheritdoc cref="Start" path="/summary"/></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary"/></param>
        public Capture ( Int32 start, Int32 length )
        {
            this.Start = start;
            this.Length = length;
        }
        
        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            obj is Capture other && this.Start == other.Start && this.Length == other.Length;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 1499269324;
            hashCode = hashCode * -1521134295 + this.Start.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.Length.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// Deconstructs this struct into it's respective parts.
        /// </summary>
        /// <param name="start"><inheritdoc cref="Start" path="/summary"/></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary"/></param>
        public void Deconstruct ( out Int32 start, out Int32 length )
        {
            start = this.Start;
            length = this.Length;
        }
    }
}