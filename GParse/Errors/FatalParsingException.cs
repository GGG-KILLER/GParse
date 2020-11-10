using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GParse.Errors
{
    /// <summary>
    /// An exception thrown when a fatal error has occurred and parsing cannot continue.
    /// </summary>
    [SuppressMessage ( "Design", "CA1032:Implement standard exception constructors", Justification = "A location or range is required." )]
    [Serializable]
    public class FatalParsingException : Exception
    {
        /// <summary>
        /// The section of code that triggered this exception
        /// </summary>
        public SourceRange Range { get; }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public FatalParsingException ( SourceLocation location, String message ) : base ( message )
        {
            if ( location is null )
                throw new ArgumentNullException ( nameof ( location ) );
            this.Range = location.To ( location );
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public FatalParsingException ( SourceRange range, String message ) : base ( message )
        {
            this.Range = range ?? throw new ArgumentNullException ( nameof ( range ) );
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParsingException ( SourceLocation location, String message, Exception innerException ) : base ( message, innerException )
        {
            if ( location is null )
                throw new ArgumentNullException ( nameof ( location ) );
            this.Range = location.To ( location );
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParsingException ( SourceRange range, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Range = range ?? throw new ArgumentNullException ( nameof ( range ) );
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException"/>
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected FatalParsingException ( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base ( serializationInfo, streamingContext )
        {
            this.Range = ( SourceRange ) ( serializationInfo.GetValue ( "FatalParsingExceptionRange", typeof ( SourceRange ) )
                ?? throw new SerializationException ( "Serialized object does not contain a Range value." ) );
        }

        /// <inheritdoc/>
        public override void GetObjectData ( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData ( info, context );
            info.AddValue ( "FatalParsingExceptionRange", this.Range, typeof ( SourceRange ) );
        }
    }
}