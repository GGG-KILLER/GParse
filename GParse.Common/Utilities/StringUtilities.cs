using System;
using System.Globalization;

namespace GParse.Common.Utilities
{
    /// <summary>
    /// General utilities for strings
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        /// Returns unprintable characters escaped as hexadecimal
        /// escape codes and printable characters as themselves
        /// </summary>
        /// <param name="chMaybe">The character.... Maybe?</param>
        /// <returns></returns>
        public static String GetCharacterRepresentation ( Char? chMaybe ) => chMaybe.HasValue
            ? GetCharacterRepresentation ( chMaybe.Value )
            : "";

        /// <summary>
        /// Returns unprintable characters escaped as hexadecimal
        /// escape codes and printable characters as themselves
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static String GetCharacterRepresentation ( Char ch )
        {
            // "\0\a\b\f\n\r\t\v"
            switch ( ch )
            {
                case '\0':
                    return "\\0";

                case '\a':
                    return "\\a";

                case '\b':
                    return "\\b";

                case '\f':
                    return "\\f";

                case '\n':
                    return "\\n";

                case '\r':
                    return "\\r";

                case '\t':
                    return "\\t";

                case '\v':
                    return "\\v";

                case ' ':
                    return " ";

                default:
                {
                    UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory ( ch );
                    return cat == UnicodeCategory.NonSpacingMark
                        || ( UnicodeCategory.SpaceSeparator <= cat && cat <= UnicodeCategory.PrivateUse )
                        || cat == UnicodeCategory.ModifierSymbol
                        || cat == UnicodeCategory.OtherNotAssigned
                        ? $"\\x{( UInt32 ) ch:X2}"
                        : ch.ToString ( );
                }
            }
        }

        /// <summary>
        /// Returns an unquoted string with unprintable characters
        /// escaped as hexadecimal escape codes and printable
        /// characters as themselves
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String GetStringRepresentation ( String input ) => input == null
                ? null
                : String.Join ( "", Array.ConvertAll ( input.ToCharArray ( ), GetCharacterRepresentation ) );
    }
}
