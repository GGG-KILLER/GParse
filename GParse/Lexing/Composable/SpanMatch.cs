#if HAS_SPAN
using System;
using System.Collections.Generic;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a match of a grammar node over a code reader
    /// </summary>
    public readonly ref struct SpanMatch
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
        public Int32 Length => this.Value.Length;

        /// <summary>
        /// The full match
        /// </summary>
        public ReadOnlySpan<Char> Value { get; }

        /// <summary>
        /// Initializes this match
        /// </summary>
        /// <param name="isMatch"></param>
        /// <param name="match"></param>
        /// <param name="captures"></param>
        internal SpanMatch ( Boolean isMatch, ReadOnlySpan<Char> match, IReadOnlyDictionary<String, (Int32 start, Int32 length)>? captures )
        {
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
        public Boolean TryGetCapture ( String name, out ReadOnlySpan<Char> span )
        {
            if ( this.IsMatch && this._captures!.TryGetValue ( name, out (Int32 start, Int32 length) value ) )
            {
                span = this.Value.Slice ( value.start, value.length );
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
#endif