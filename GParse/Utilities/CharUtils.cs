using System;
using System.Runtime.CompilerServices;

namespace GParse.Utilities
{
    /// <summary>
    /// A class containing utility methods for <see cref="Char"/>s.
    /// </summary>
    public static class CharUtils
    {
        /// <summary>
        /// Checks whether the provided <paramref name="value" /> is in the range [<paramref
        /// name="start" />, <paramref name="end" />].
        /// </summary>
        /// <param name="start">The first character of the range (inclusive).</param>
        /// <param name="value">The character to check for.</param>
        /// <param name="end">The last character of the range (inclusive).</param>
        /// <returns>Whether the provided character is in the range.</returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Boolean IsInRange ( Char start, Char value, Char end ) =>
            ( UInt32 ) ( value - start ) <= ( end - start );

        /// <summary>
        /// Converts the provided ASCII character into lower-case ASCII.
        /// </summary>
        /// <param name="ch">The character to convert.</param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static Char AsciiLowerCase ( Char ch ) =>
            ( Char ) ( ch | 0b100000 );

        /// <summary>
        /// Converts a character into a readable (possibly escaped) string representation.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static String ToReadableString ( Char ch ) =>
            ch == ' ' || Char.IsLetterOrDigit ( ch ) || Char.IsPunctuation ( ch )
                ? Char.ToString ( ch )
                : $"\\u{( UInt16 ) ch:X4}";

    }
}
