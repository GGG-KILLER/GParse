using System;
using System.Collections.Generic;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Capture : Node, IEquatable<Capture>
    {
        public readonly Node Inner;
        public readonly Int32 CaptureNumber;

        public Capture ( Int32 number, Node @internal )
        {
            this.CaptureNumber = number;
            this.Inner = @internal;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Capture other ) => other != null && this.IsLazy == other.IsLazy && EqualityComparer<Node>.Default.Equals ( this.Inner, other.Inner ) && this.CaptureNumber == other.CaptureNumber;

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Capture );

        public override Int32 GetHashCode ( )
        {
            var hashCode = 828655947;
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode ( this.Inner );
            hashCode = hashCode * -1521134295 + this.CaptureNumber.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( Capture capture1, Capture capture2 ) => EqualityComparer<Capture>.Default.Equals ( capture1, capture2 );

        public static Boolean operator != ( Capture capture1, Capture capture2 ) => !( capture1 == capture2 );

        #endregion Object
    }
}
