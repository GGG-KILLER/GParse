using System;
using System.Collections.Generic;

namespace GParse.Common
{
    public readonly struct SourceRange : IEquatable<SourceRange>
    {
        public static readonly SourceRange Zero = new SourceRange ( SourceLocation.Zero, SourceLocation.Zero );

        public readonly SourceLocation Start;
        public readonly SourceLocation End;

        public SourceRange ( SourceLocation start, SourceLocation end )
        {
            this.End = end;
            this.Start = start;
        }

        public override String ToString ( ) => $"{this.Start} - {this.End}";

        #region Generated Code

        public override Boolean Equals ( Object obj ) => obj is SourceRange && this.Equals ( ( SourceRange ) obj );

        public Boolean Equals ( SourceRange other ) => this.End.Equals ( other.End )
                    && this.Start.Equals ( other.Start );

        public override Int32 GetHashCode ( )
        {
            var hashCode = 945720665;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.End );
            return ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.Start );
        }

        public static Boolean operator == ( SourceRange lhs, SourceRange rhs ) => lhs.Equals ( rhs );

        public static Boolean operator != ( SourceRange lhs, SourceRange rhs ) => !( lhs == rhs );

        #endregion Generated Code
    }
}
