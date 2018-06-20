using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public sealed class RulePlaceholder : BaseMatcher, IEquatable<RulePlaceholder>
    {
        internal readonly String Name;

        public RulePlaceholder ( String Name )
        {
            this.Name = Name;
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as RulePlaceholder );
        }

        public Boolean Equals ( RulePlaceholder other )
        {
            return other != null &&
                    EqualityComparer<VerboseParser>.Default.Equals ( this.Parser, other.Parser ) &&
                     this.Name == other.Name;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1940561674;
            hashCode = hashCode * -1521134295 + EqualityComparer<VerboseParser>.Default.GetHashCode ( this.Parser );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            return hashCode;
        }

        public static Boolean operator == ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => EqualityComparer<RulePlaceholder>.Default.Equals ( placeholder1, placeholder2 );

        public static Boolean operator != ( RulePlaceholder placeholder1, RulePlaceholder placeholder2 ) => !( placeholder1 == placeholder2 );

        #endregion Generated Code
    }
}
