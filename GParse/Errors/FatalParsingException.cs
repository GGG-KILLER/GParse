﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Errors
{
    /// <summary>
    /// Represents a kind of error that the parser and/or lexer cannot ignore.
    /// </summary>
    [SuppressMessage ( "Design", "CA1032:Implement standard exception constructors", Justification = "This type of exception is not meant to be built without location information." )]
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
            this.Range = location.To ( location );
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public FatalParsingException ( SourceRange range, String message ) : base ( message )
        {
            this.Range = range;
        }

        /// <summary>
        /// Initializes a new <see cref="FatalParsingException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalParsingException ( SourceLocation location, String message, Exception innerException ) : base ( message, innerException )
        {
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
            this.Range = range;
        }
    }
}
