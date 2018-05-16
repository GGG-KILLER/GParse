using System;

namespace GParse.Common
{
    public struct SourceLocation : IEquatable<SourceLocation>
    {
        public static readonly SourceLocation Zero = new SourceLocation ( 0, 0, 0 );
        public static readonly SourceLocation Max = new SourceLocation ( Int32.MaxValue, Int32.MaxValue, Int32.MaxValue );
        public static readonly SourceLocation Min = new SourceLocation ( Int32.MinValue, Int32.MinValue, Int32.MinValue );

        public Int32 Column;
        public Int32 Line;

        /// <summary>
        /// The byte of this location
        /// </summary>
        public Int32 Byte;

        public SourceLocation ( Int32 line, Int32 column, Int32 pos )
        {
            this.Line = line;
            this.Column = column;
            this.Byte = pos;
        }

        public SourceRange To ( SourceLocation end )
        {
            return new SourceRange ( this, end );
        }

        public override String ToString ( )
        {
            return $"{this.Line}:{this.Column}";
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return obj is SourceLocation && this.Equals ( ( SourceLocation ) obj );
        }

        public Boolean Equals ( SourceLocation other )
        {
            return this.Column == other.Column
                     && this.Line == other.Line;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 412437926;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + this.Column.GetHashCode ( );
            return ( hashCode * -1521134295 ) + this.Line.GetHashCode ( );
        }

        public static Boolean operator == ( SourceLocation lhs, SourceLocation rhs )
        {
            return lhs.Equals ( rhs );
        }

        public static Boolean operator != ( SourceLocation lhs, SourceLocation rhs )
        {
            return !( lhs == rhs );
        }

        #endregion Generated Code
    }
}
