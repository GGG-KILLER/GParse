using System;
using System.Globalization;

namespace GParse.Verbose.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// A bitmask with all nonprintable character categories
        /// </summary>
        public static readonly UnicodeCategory[] NonPrintableCategories = new[]{
            UnicodeCategory.Control, UnicodeCategory.Format, UnicodeCategory.LineSeparator, UnicodeCategory.ModifierSymbol,
            UnicodeCategory.NonSpacingMark, UnicodeCategory.OtherNotAssigned, UnicodeCategory.Surrogate, UnicodeCategory.ParagraphSeparator,
            UnicodeCategory.PrivateUse, UnicodeCategory.SpaceSeparator
        };

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
                    return Array.IndexOf ( NonPrintableCategories, CharUnicodeInfo.GetUnicodeCategory ( ch ) ) != -1
                        ? $"\\x{( Int32 ) ch:X4}"
                        : ch.ToString ( );
            }
        }

        /// <summary>
        /// Returns an unquoted string with unprintable characters
        /// escaped as hexadecimal escape codes and printable
        /// characters as themselves
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String GetStringRepresentation ( String input )
        {
            return String.Join ( "", Array.ConvertAll ( input.ToCharArray ( ), GetCharacterRepresentation ) );
        }
    }
}
