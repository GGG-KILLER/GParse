using System;

namespace GParse.Common
{
    public struct SourceLocation : IEquatable<SourceLocation>
    {
        public static readonly SourceLocation Zero = new SourceLocation ( 1, 1, 0 );
        public static readonly SourceLocation Max = new SourceLocation ( Int32.MaxValue, Int32.MaxValue, Int32.MaxValue );
        public static readonly SourceLocation Min = new SourceLocation ( Int32.MinValue, Int32.MinValue, Int32.MinValue );

        public readonly Int32 Byte;
        public readonly Int32 Line;
        public readonly Int32 Column;

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

        public void Deconstruct ( out Int32 Line, out Int32 Column )
        {
            Line = this.Line;
            Column = this.Column;
        }

        public void Deconstruct ( out Int32 Line, out Int32 Column, out Int32 Byte )
        {
            Line = this.Line;
            Column = this.Column;
            Byte = this.Byte;
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
