using System;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a capture.
    /// </summary>
    public readonly struct Capture : IEquatable<Capture>
    {
        /// <summary>
        /// The starting position of the capture.
        /// </summary>
        public Int32 Start { get; }

        /// <summary>
        /// The length of the capture.
        /// </summary>
        public Int32 Length { get; }

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

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => obj is Capture capture && this.Equals ( capture );

        /// <inheritdoc/>
        public Boolean Equals ( Capture other ) => this.Start == other.Start && this.Length == other.Length;

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) => HashCode.Combine ( this.Start, this.Length );

        /// <summary>
        /// Checks whether two captures are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( Capture left, Capture right ) => left.Equals ( right );

        /// <summary>
        /// Checks whether two captures are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Capture left, Capture right ) => !( left == right );
    }
}