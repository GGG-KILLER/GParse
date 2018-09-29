using System;
using System.Collections.Generic;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class CaptureReference : Node, IEquatable<CaptureReference>
    {
        public readonly Int32 CaptureNumber;

        public CaptureReference ( Int32 captureNumber )
        {
            this.CaptureNumber = captureNumber;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( CaptureReference other ) => other != null && this.IsLazy == other.IsLazy && this.CaptureNumber == other.CaptureNumber;

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as CaptureReference );

        public override Int32 GetHashCode ( ) => 154084684 + this.CaptureNumber.GetHashCode ( );

        public static Boolean operator == ( CaptureReference reference1, CaptureReference reference2 ) => EqualityComparer<CaptureReference>.Default.Equals ( reference1, reference2 );

        public static Boolean operator != ( CaptureReference reference1, CaptureReference reference2 ) => !( reference1 == reference2 );

        #endregion Object
    }
}
