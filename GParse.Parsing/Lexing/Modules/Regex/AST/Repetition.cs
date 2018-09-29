using System;
using System.Collections.Generic;
using GParse.Common.Math;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Repetition : Node, IEquatable<Repetition>
    {
        public readonly Range<UInt32> Range;
        public readonly Node Inner;

        public Repetition ( Range<UInt32> range, Node @internal )
        {
            this.Range = range;
            this.Inner = @internal;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Repetition other ) => other != null && this.IsLazy == other.IsLazy && this.Range.Equals ( other.Range ) && EqualityComparer<Node>.Default.Equals ( this.Inner, other.Inner );

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Repetition );

        public override Int32 GetHashCode ( )
        {
            var hashCode = -517187063;
            hashCode = hashCode * -1521134295 + EqualityComparer<Range<UInt32>>.Default.GetHashCode ( this.Range );
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode ( this.Inner );
            return hashCode;
        }

        public static Boolean operator == ( Repetition repetition1, Repetition repetition2 ) => EqualityComparer<Repetition>.Default.Equals ( repetition1, repetition2 );

        public static Boolean operator != ( Repetition repetition1, Repetition repetition2 ) => !( repetition1 == repetition2 );

        #endregion Object
    }
}
