using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Sequence : Node, IEquatable<Sequence>
    {
        public readonly Node[] Children;

        public Sequence ( IEnumerable<Node> children )
        {
            if ( children == null )
                throw new ArgumentNullException ( nameof ( children ) );

            this.Children = children.ToArray ( );
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Sequence other ) => other != null && this.IsLazy == other.IsLazy && this.Children.SequenceEqual ( other.Children );

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Sequence );

        public override Int32 GetHashCode ( ) => 51172802 + EqualityComparer<Node[]>.Default.GetHashCode ( this.Children );

        public static Boolean operator == ( Sequence sequence1, Sequence sequence2 ) => EqualityComparer<Sequence>.Default.Equals ( sequence1, sequence2 );

        public static Boolean operator != ( Sequence sequence1, Sequence sequence2 ) => !( sequence1 == sequence2 );

        #endregion Object
    }
}
