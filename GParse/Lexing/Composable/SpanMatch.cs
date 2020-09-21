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
        /// The capture groups.
        /// </summary>
        private readonly IReadOnlyDictionary<String, Capture>? _captures;

        /// <summary>
        /// Whether this match was successful.
        /// </summary>
        public Boolean IsMatch { get; }

        /// <summary>
        /// The length of the match.
        /// </summary>
        public Int32 Length => this.Value.Length;

        /// <summary>
        /// The full match.
        /// </summary>
        public ReadOnlySpan<Char> Value { get; }

        /// <summary>
        /// Initializes a new span-backed match.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="captures">The captures saved during matching.</param>
        internal SpanMatch ( Boolean isMatch, ReadOnlySpan<Char> value, IReadOnlyDictionary<String, Capture>? captures )
        {
            this.IsMatch = isMatch;
            this.Value = value;
            this._captures = captures;
        }

        /// <summary>
        /// Attempts to get the value of a named capture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public Boolean TryGetCaptureText ( String name, out ReadOnlySpan<Char> span )
        {
            if ( this.IsMatch && this._captures!.TryGetValue ( name, out Capture value ) )
            {
                span = this.Value.Slice ( value.Start, value.Length );
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