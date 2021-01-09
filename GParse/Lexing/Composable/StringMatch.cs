using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a match of a grammar node over a code reader
    /// </summary>
    [SuppressMessage ( "Performance", "CA1815:Override equals and operator equals on value types", Justification = "These shouldn't ever be compared to each other." )]
    public readonly struct StringMatch
    {
        /// <summary>
        /// The capture groups
        /// </summary>
        private readonly IReadOnlyDictionary<String, Capture>? _captures;

        /// <summary>
        /// Whether this match was successful
        /// </summary>
        [MemberNotNullWhen ( true, nameof ( Value ) )]
        public Boolean IsMatch { get; }

        /// <summary>
        /// The length of the match. Will throw if <see cref="IsMatch"/> is <see langword="false"/>.
        /// </summary>
        public Int32 Length => this.Value!.Length;

        /// <summary>
        /// The full match. Only null when <see cref="IsMatch"/> is <see langword="false"/>.
        /// </summary>
        public String? Value { get; }

        /// <summary>
        /// Initializes this match
        /// </summary>
        /// <param name="isMatch"></param>
        /// <param name="match"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="captures">The named capture ranges (may only be null if the match wasn't successful)</param>
        internal StringMatch ( Boolean isMatch, String? match, IReadOnlyDictionary<String, Capture>? captures )
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
        public Boolean TryGetCaptureText ( String name, [NotNullWhen ( true )] out String? span )
        {
            if ( this.IsMatch && this._captures!.TryGetValue ( name, out Capture value ) )
            {
                span = this.Value.Substring ( value.Start, value.Length );
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public override String ToString ( ) => $"StringMatch {{ IsMatch = {this.IsMatch}, Length = {this.Length} }}";
    }
}