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
        private readonly IReadOnlyDictionary<string, Capture>? _captures;

        /// <summary>
        /// Whether this match was successful.
        /// </summary>
        public bool IsMatch { get; }

        /// <summary>
        /// The length of the match.
        /// </summary>
        public int Length => Value.Length;

        /// <summary>
        /// The full match.
        /// </summary>
        public ReadOnlySpan<char> Value { get; }

        /// <summary>
        /// Initializes a new span-backed match.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="captures">The captures saved during matching.</param>
        internal SpanMatch(bool isMatch, ReadOnlySpan<char> value, IReadOnlyDictionary<string, Capture>? captures)
        {
            IsMatch = isMatch;
            Value = value;
            _captures = captures;
        }

        /// <summary>
        /// Attempts to get the value of a named capture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public bool TryGetCaptureText(string name, out ReadOnlySpan<char> span)
        {
            if (IsMatch && _captures!.TryGetValue(name, out var value))
            {
                span = Value.Slice(value.Start, value.Length);
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