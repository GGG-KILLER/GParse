using System;
using System.Collections.Generic;
using GParse.Common.Math;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Range : Node, IEquatable<Range>
    {
        public readonly Range<Char> CharRange;

        public Range ( Range<Char> range )
        {
            this.CharRange = range;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Range other ) => other != null && this.IsLazy == other.IsLazy && this.CharRange.Equals ( other.CharRange );

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Range );

        public override Int32 GetHashCode ( )
        {
            var hashCode = 248606272;
            hashCode = hashCode * -1521134295 + base.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Range<Char>>.Default.GetHashCode ( this.CharRange );
            return hashCode;
        }

        public static Boolean operator == ( Range range1, Range range2 ) => EqualityComparer<Range>.Default.Equals ( range1, range2 );

        public static Boolean operator != ( Range range1, Range range2 ) => !( range1 == range2 );

        #endregion Object
    }
}
