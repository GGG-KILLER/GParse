using System;
using System.Collections.Generic;

namespace GParse.Common
{
    /// <summary>
    /// Defines a range in source code
    /// </summary>
    public readonly struct SourceRange : IEquatable<SourceRange>
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly SourceRange Zero = new SourceRange ( SourceLocation.Zero, SourceLocation.Zero );

        /// <summary>
        /// Starting location
        /// </summary>
        public readonly SourceLocation Start;

        /// <summary>
        /// Ending location
        /// </summary>
        public readonly SourceLocation End;

        /// <summary>
        /// Initializes this range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public SourceRange ( SourceLocation start, SourceLocation end )
        {
            this.End = end;
            this.Start = start;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) => $"{this.Start} - {this.End}";

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is SourceRange && this.Equals ( ( SourceRange ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( SourceRange other ) => this.End.Equals ( other.End )
                    && this.Start.Equals ( other.Start );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 945720665;
            hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.End );
            return ( hashCode * -1521134295 ) + EqualityComparer<SourceLocation>.Default.GetHashCode ( this.Start );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator == ( SourceRange lhs, SourceRange rhs ) => lhs.Equals ( rhs );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean operator != ( SourceRange lhs, SourceRange rhs ) => !( lhs == rhs );

        #endregion Generated Code
    }
}
