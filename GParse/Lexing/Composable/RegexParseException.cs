using System;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// An exception raised while parsing a regex expression.
    /// </summary>
    public class RegexParseException : Exception
    {
        /// <summary>
        /// The range of offsets this error refers to.
        /// </summary>
        public Range<Int32> Range { get; }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public RegexParseException ( Int32 location, String message ) : base ( message )
        {
            this.Range = new Range<Int32> ( location, location + 1 );
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public RegexParseException ( Range<Int32> range, String message ) : base ( message )
        {
            this.Range = range;
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="location"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"><inheritdoc/></param>
        public RegexParseException ( Int32 location, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Range = new Range<Int32> ( location );
        }

        /// <summary>
        /// Initializes a new regex parse exception.
        /// </summary>
        /// <param name="range"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"></param>
        public RegexParseException ( Range<Int32> range, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Range = range;
        }
    }
}
