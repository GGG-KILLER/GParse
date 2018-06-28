using System;
using System.Collections.Generic;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public sealed class RulePlaceholder : BaseMatcher, IEquatable<RulePlaceholder>
    {
        public readonly String Name;

        public RulePlaceholder ( String Name )
        {
            this.Name = Name;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as RulePlaceholder );
        }

        public Boolean Equals ( RulePlaceholder other )
        {
            return other != null &&
                     this.Name == other.Name;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1940561674;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            return hashCode;
        }

        public static Boolean operator == ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => EqualityComparer<RulePlaceholder>.Default.Equals ( placeholder1, placeholder2 );

        public static Boolean operator != ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => !( placeholder1 == placeholder2 );

        #endregion Generated Code
    }
}
