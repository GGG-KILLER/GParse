using System;
using System.Collections.Generic;

namespace GParse.Parsing.Settings
{
    /// <summary>
    /// The settings to use when lexing a string. These same
    /// settings are used for chars but
    /// <see cref="NewlineEscape" /> is ignored.
    /// </summary>
    public class StringLexSettings : IEquatable<StringLexSettings>
    {
        /// <summary>
        /// The settings used for individual characters in the string
        /// </summary>
        public CharLexSettings CharSettings;

        /// <summary>
        /// The escape code for new lines thus making the string multi-line
        /// </summary>
        public String NewlineEscape;

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as StringLexSettings );
        }

        public Boolean Equals ( StringLexSettings other )
        {
            return other != null
                    && EqualityComparer<CharLexSettings>.Default.Equals ( this.CharSettings, other.CharSettings )
                     && this.NewlineEscape == other.NewlineEscape;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 1387991148;
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<CharLexSettings>.Default.GetHashCode ( this.CharSettings );
            return ( hashCode * -1521134295 ) + EqualityComparer<String>.Default.GetHashCode ( this.NewlineEscape );
        }

        public static Boolean operator == ( StringLexSettings settings1, StringLexSettings settings2 )
        {
            return EqualityComparer<StringLexSettings>.Default.Equals ( settings1, settings2 );
        }

        public static Boolean operator != ( StringLexSettings settings1, StringLexSettings settings2 )
        {
            return !( settings1 == settings2 );
        }

        #endregion Generated Code
    }
}
