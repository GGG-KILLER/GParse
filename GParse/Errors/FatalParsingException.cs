using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using GParse.Math;

namespace GParse.Errors
{
    /// <summary>
    /// An exception thrown when a fatal error has occurred and parsing cannot continue.
    /// </summary>
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "A location or range is required.")]
    [Serializable]
    public class FatalParsingException : Exception
    {
        /// <summary>
        /// The section of code that triggered this exception
        /// </summary>
        public Range<int> Range { get; }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public FatalParsingException(int location, string message) : base(message)
        {
            Range = new Range<int>(location);
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public FatalParsingException(Range<int> range, string message) : base(message)
        {
            Range = range;
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParsingException(int location, string message, Exception innerException) : base(message, innerException)
        {
            Range = new Range<int>(location);
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParsingException(Range<int> range, string message, Exception innerException) : base(message, innerException)
        {
            Range = range;
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException"/>
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected FatalParsingException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            var deserialized = serializationInfo.GetValue("FatalParsingExceptionRange", typeof(Range<int>))
                ?? throw new SerializationException("Serialized object does not contain a Range value.");
            Range = (Range<int>) deserialized;
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FatalParsingExceptionRange", Range, typeof(Range<int>));
        }
    }
}
