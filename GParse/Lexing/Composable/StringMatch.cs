using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a match of a grammar node over a code reader
    /// </summary>
    public readonly struct StringMatch
    {
        /// <summary>
        /// The capture groups
        /// </summary>
        private readonly IReadOnlyDictionary<String, (Int32 start, Int32 length)>? _captures;

        /// <summary>
        /// Whether this match was successful
        /// </summary>
        public Boolean IsMatch { get; }

        /// <summary>
        /// The length of the match
        /// </summary>
        public Int32 Length => this.Value!.Length;

        /// <summary>
        /// The full match
        /// </summary>
        public String? Value { get; }

        /// <summary>
        /// Initializes this match
        /// </summary>
        /// <param name="isMatch"></param>
        /// <param name="match">The full match (may only be null if the match wasn't successful)</param>
        /// <param name="captures">The named capture ranges (may only be null if the match wasn't successful)</param>
        internal StringMatch ( Boolean isMatch, String? match, IReadOnlyDictionary<String, (Int32 start, Int32 length)>? captures )
        {
            if ( isMatch && match is null )
                throw new ArgumentNullException ( nameof ( match ) );
            if ( isMatch && captures is null )
                throw new ArgumentNullException ( nameof ( captures ) );

            this.IsMatch = isMatch;
            this.Value = match;
            this._captures = captures;
        }

        /// <summary>
        /// Attempts to get the value of a named capture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public Boolean TryGetCapture ( String name, [NotNullWhen ( true )] out String? span )
        {
            if ( this.IsMatch && this._captures!.TryGetValue ( name, out (Int32 start, Int32 length) value ) )
            {
                span = this.Value!.Substring ( value.start, value.length );
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }
    }
}