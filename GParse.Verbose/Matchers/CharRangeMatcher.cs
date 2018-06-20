using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public sealed class CharRangeMatcher : BaseMatcher, IEquatable<CharRangeMatcher>
    {
        internal readonly Boolean Strict;
        internal readonly Char Start;
        internal readonly Char End;

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        /// <param name="Strict">
        /// Whether to use Start &lt; value &lt; End instead of
        /// Start ≤ value ≤ End
        /// </param>
        public CharRangeMatcher ( Char Start, Char End, Boolean Strict )
        {
            this.Start = ( Char ) Math.Min ( Start, End );
            this.End = ( Char ) Math.Max ( Start, End );
            this.Strict = Strict;

            if ( !this.Strict )
            {
                this.Start--;
                this.End++;
            }
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as CharRangeMatcher );
        }

        public Boolean Equals ( CharRangeMatcher other )
        {
            return other != null &&
                     this.Strict == other.Strict &&
                     this.Start == other.Start &&
                     this.End == other.End;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -2061379221;
            hashCode = hashCode * -1521134295 + this.Strict.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.Start.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.End.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => EqualityComparer<CharRangeMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( CharRangeMatcher matcher1, CharRangeMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
