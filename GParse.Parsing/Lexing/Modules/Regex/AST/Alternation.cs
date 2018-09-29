using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Alternation : Node, IEquatable<Alternation>
    {
        public readonly Boolean IsNegated;
        public readonly Node[] Children;

        public Alternation ( IEnumerable<Node> children, Boolean isNegated )
        {
            if ( children == null )
                throw new ArgumentNullException ( nameof ( children ) );

            this.Children = children.ToArray ( );
            this.IsNegated = isNegated;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Alternation other ) => other != null && this.IsLazy == other.IsLazy && this.IsNegated == other.IsNegated && this.Children.SequenceEqual ( other.Children );

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Alternation );

        public override Int32 GetHashCode ( )
        {
            var hashCode = 1718947575;
            hashCode = hashCode * -1521134295 + this.IsNegated.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Node[]>.Default.GetHashCode ( this.Children );
            return hashCode;
        }

        public static Boolean operator == ( Alternation alternation1, Alternation alternation2 ) => EqualityComparer<Alternation>.Default.Equals ( alternation1, alternation2 );

        public static Boolean operator != ( Alternation alternation1, Alternation alternation2 ) => !( alternation1 == alternation2 );

        #endregion Object
    }
}
