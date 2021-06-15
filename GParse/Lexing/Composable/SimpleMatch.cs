using System;
using System.Runtime.CompilerServices;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A simple match containing only whether the match was successful and its length if successful.
    /// </summary>
    public readonly struct SimpleMatch : IEquatable<SimpleMatch>
    {
        /// <summary>
        /// A match result that indicates there was no match.
        /// </summary>
        public static SimpleMatch Fail
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
        }

        /// <summary>
        /// A match result that indicates there was a match of length of a single char.
        /// </summary>
        public static SimpleMatch SingleChar
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(true, 1);
        }

        /// <summary>
        /// Whether this match was successful.
        /// </summary>
        public bool IsMatch { get; }

        /// <summary>
        /// The length of the match.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Initializes a new simple match.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary" /></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary" /></param>
        public SimpleMatch(bool isMatch, int length)
        {
            IsMatch = isMatch;
            Length = length;
        }

        /// <summary>
        /// Deconstructs this simple match into its counterparts.
        /// </summary>
        /// <param name="isMatch"><inheritdoc cref="IsMatch" path="/summary" /></param>
        /// <param name="length"><inheritdoc cref="Length" path="/summary" /></param>
        public void Deconstruct(out bool isMatch, out int length)
        {
            isMatch = IsMatch;
            length = Length;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is SimpleMatch match && Equals(match);
        /// <inheritdoc/>
        public bool Equals(SimpleMatch other) => IsMatch == other.IsMatch && Length == other.Length;
        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(IsMatch, Length);

        /// <summary>
        /// Checks whether two simple matches are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SimpleMatch left, SimpleMatch right) => left.Equals(right);

        /// <summary>
        /// Checks whether two simple matches are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SimpleMatch left, SimpleMatch right) => !(left == right);
    }
}