using System;
using System.Collections.Generic;

namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal class Literal : Node, IEquatable<Literal>
    {
        public readonly Char Value;

        public Literal ( Char value )
        {
            this.Value = value;
        }

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region IEquatable<T>

        public Boolean Equals ( Literal other ) => other != null && this.IsLazy == other.IsLazy && this.Value == other.Value;

        #endregion IEquatable<T>
        #region Object

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as Literal );

        public override Int32 GetHashCode ( ) => -1937169414 + EqualityComparer<Char>.Default.GetHashCode ( this.Value );

        public static Boolean operator == ( Literal literal1, Literal literal2 ) => EqualityComparer<Literal>.Default.Equals ( literal1, literal2 );

        public static Boolean operator != ( Literal literal1, Literal literal2 ) => !( literal1 == literal2 );

        #endregion Object
    }
}
