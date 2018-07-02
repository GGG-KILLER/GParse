using System;
using System.Collections.Generic;
using GParse.Verbose.Parsing.Abstractions;

namespace GParse.Verbose.Parsing.Matchers
{
    public sealed class RuleWrapper : MatcherWrapper, IEquatable<RuleWrapper>
    {
        public readonly String Name;

        public RuleWrapper ( BaseMatcher Matcher, String Name )
            : base ( Matcher )
        {
            this.Name = Name;
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as RuleWrapper );
        }

        public Boolean Equals ( RuleWrapper other )
        {
            return other != null &&
                     this.Name == other.Name;
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 593797347;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            return hashCode;
        }

        public static Boolean operator == ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => EqualityComparer<RuleWrapper>.Default.Equals ( wrapper1, wrapper2 );

        public static Boolean operator != ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => !( wrapper1 == wrapper2 );

        #endregion Generated Code
    }
}
