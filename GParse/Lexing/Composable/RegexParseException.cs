using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An exception raised while parsing a regex expression.
    /// </summary>
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Class shouldn't be constructed without a range.")]
    [Serializable]
    public sealed class RegexParseException : Exception
    {
        /// <summary>
        /// The range of offsets this error refers to.
        /// </summary>
        public Range<Int32> Range { get; }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location">The location where the error happened.</param>
        /// <param name="message">The error message.</param>
        public RegexParseException(Int32 location, String message) : base(message)
        {
            this.Range = new Range<Int32>(location, location + 1);
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range">The range where the error happened.</param>
        /// <param name="message"><inheritdoc cref="RegexParseException(Int32, String)"/></param>
        public RegexParseException(Range<Int32> range, String message) : base(message)
        {
            this.Range = range;
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location"><inheritdoc cref="RegexParseException(Int32, String)"/></param>
        /// <param name="message"><inheritdoc cref="RegexParseException(Int32, String)"/></param>
        /// <param name="innerException"><inheritdoc cref="Exception(String, Exception)"/></param>
        public RegexParseException(Int32 location, String message, Exception innerException) : base(message, innerException)
        {
            this.Range = new Range<Int32>(location);
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range"><inheritdoc cref="RegexParseException(Range{Int32}, String)"/></param>
        /// <param name="message"><inheritdoc cref="RegexParseException(Range{Int32}, String)"/></param>
        /// <param name="innerException"><inheritdoc cref="Exception(String, Exception)"/></param>
        public RegexParseException(Range<Int32> range, String message, Exception innerException) : base(message, innerException)
        {
            this.Range = range;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParseException"/> class with serialized data.
        /// </summary>
        /// <param name="serializationInfo"><inheritdoc cref="RegexParseException(SerializationInfo, StreamingContext)"/></param>
        /// <param name="streamingContext"><inheritdoc cref="RegexParseException(SerializationInfo, StreamingContext)"/></param>
        private RegexParseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            if (serializationInfo.GetValue("RegexParseException.Range", typeof(Range<Int32>)) is not Range<Int32> range)
                throw new SerializationException("Range field is missing.");
            this.Range = range;
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RegexParseException.Range", this.Range, typeof(Range<Int32>));
        }
    }
}