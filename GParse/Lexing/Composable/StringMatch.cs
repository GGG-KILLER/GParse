using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a match of a grammar node over a code reader
    /// </summary>
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "These shouldn't ever be compared to each other.")]
    public readonly struct StringMatch
    {
        /// <summary>
        /// The capture groups
        /// </summary>
        private readonly IReadOnlyDictionary<string, Capture>? _captures;

        /// <summary>
        /// Whether this match was successful
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsMatch { get; }

        /// <summary>
        /// The length of the match. Will throw if <see cref="IsMatch"/> is <see langword="false"/>.
        /// </summary>
        public int Length => Value!.Length;

        /// <summary>
        /// The full match. Only null when <see cref="IsMatch"/> is <see langword="false"/>.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Initializes this match
        /// </summary>
        /// <param name="isMatch"></param>
        /// <param name="match"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="captures">The named capture ranges (may only be null if the match wasn't successful)</param>
        internal StringMatch(bool isMatch, string? match, IReadOnlyDictionary<string, Capture>? captures)
        {
            if (isMatch && match is null)
                throw new ArgumentNullException(nameof(match));
            if (isMatch && captures is null)
                throw new ArgumentNullException(nameof(captures));

            IsMatch = isMatch;
            Value = match;
            _captures = captures;
        }

        /// <summary>
        /// Attempts to get the value of a named capture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public bool TryGetCaptureText(string name, [NotNullWhen(true)] out string? span)
        {
            if (IsMatch && _captures!.TryGetValue(name, out var value))
            {
                span = Value.Substring(value.Start, value.Length);
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"StringMatch {{ IsMatch = {IsMatch}, Length = {Length} }}";
    }
}