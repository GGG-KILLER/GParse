using System;
using System.Collections.Generic;

namespace GParse.Parsing.Settings
{
    /// <summary>
    /// Settings used when lexing a character
    /// </summary>
    public class CharLexSettings : IEquatable<CharLexSettings>
    {
        /// <summary>
        /// The empty <see cref="CharLexSettings" />
        /// </summary>
        public static readonly CharLexSettings Empty;

        /// <summary>
        /// The prefix of binary escapes
        /// </summary>
        public String BinaryEscapePrefix;

        /// <summary>
        /// Maximum length (in chars) of the binary escape
        /// </summary>
        public Int32 BinaryEscapeMaxLengh;

        /// <summary>
        /// Constant escapes that require no parsing
        /// </summary>
        public readonly Dictionary<String, Char> ConstantEscapes = new Dictionary<String, Char> ( );

        /// <summary>
        /// The prefix of decimal escapes
        /// </summary>
        public String DecimalEscapePrefix;

        /// <summary>
        /// Maximum length (in chars) of the decimal escape
        /// </summary>
        public Int32 DecimalEscapeMaxLengh;

        /// <summary>
        /// Maximum value of a character in the decimal escape
        /// </summary>
        public Int32 DecimalEscapeMaxValue;

        /// <summary>
        /// The prefix for hexadecimal escapes
        /// </summary>
        public String HexadecimalEscapePrefix;

        /// <summary>
        /// Maximum length (in chars) of the hexadecimal escape
        /// </summary>
        public Int32 HexadecimalEscapeMaxLengh;

        /// <summary>
        /// The prefix of octal escapes
        /// </summary>
        public String OctalEscapePrefix;

        /// <summary>
        /// Maximum length (in chars) of the octal escape
        /// </summary>
        public Int32 OctalEscapeMaxLengh;

        public CharLexSettings RegisterEscapeConstant ( String escape, Char value )
        {
            this.ConstantEscapes.Add ( escape, value );
            return this;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as CharLexSettings );

        public Boolean Equals ( CharLexSettings other ) => other != null && this.BinaryEscapePrefix == other.BinaryEscapePrefix && this.BinaryEscapeMaxLengh == other.BinaryEscapeMaxLengh && EqualityComparer<Dictionary<String, Char>>.Default.Equals ( this.ConstantEscapes, other.ConstantEscapes ) && this.DecimalEscapePrefix == other.DecimalEscapePrefix && this.DecimalEscapeMaxLengh == other.DecimalEscapeMaxLengh && this.HexadecimalEscapePrefix == other.HexadecimalEscapePrefix && this.HexadecimalEscapeMaxLengh == other.HexadecimalEscapeMaxLengh && this.OctalEscapePrefix == other.OctalEscapePrefix && this.OctalEscapeMaxLengh == other.OctalEscapeMaxLengh;

        public override Int32 GetHashCode ( )
        {
            var hashCode = -223335905;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.BinaryEscapePrefix );
            hashCode = hashCode * -1521134295 + this.BinaryEscapeMaxLengh.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<String, Char>>.Default.GetHashCode ( this.ConstantEscapes );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.DecimalEscapePrefix );
            hashCode = hashCode * -1521134295 + this.DecimalEscapeMaxLengh.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.HexadecimalEscapePrefix );
            hashCode = hashCode * -1521134295 + this.HexadecimalEscapeMaxLengh.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.OctalEscapePrefix );
            hashCode = hashCode * -1521134295 + this.OctalEscapeMaxLengh.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( CharLexSettings settings1, CharLexSettings settings2 ) => EqualityComparer<CharLexSettings>.Default.Equals ( settings1, settings2 );

        public static Boolean operator != ( CharLexSettings settings1, CharLexSettings settings2 ) => !( settings1 == settings2 );

        #endregion Generated Code
    }
}
