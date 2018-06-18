using System;
using System.Collections.Generic;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Exceptions;

namespace GParse.Verbose.Matchers
{
    public sealed class RuleWrapper : MatcherWrapper, IEquatable<RuleWrapper>
    {
        internal readonly String Name;
        internal readonly Action<String, String[]> RuleMatched;
        internal readonly Action<String> RuleExit;
        internal readonly Action<String> RuleEnter;

        public RuleWrapper ( BaseMatcher Matcher, String Name, Action<String> RuleEnter, Action<String, String[]> RuleMatched, Action<String> RuleExit )
            : base ( Matcher )
        {
            this.Name = Name;
            // We need these as delegates because when
            // everything's compiled into expression trees we
            // won't have instances to call events on. (future
            // proofing™ (actually I tried to do it once but
            // failed, but shhhh))
            this.RuleEnter = RuleEnter;
            this.RuleExit = RuleExit;
            this.RuleMatched = RuleMatched;
        }

        public override Int32 MatchLength ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return base.MatchLength ( reader, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            try
            {
                this.RuleEnter ( this.Name );
                this.RuleMatched ( this.Name, base.Match ( reader ) );
                return Array.Empty<String> ( );
            }
            catch ( Exception ex )
            {
                throw new RuleFailureException ( reader.Location, this.Name, ex );
            }
            finally
            {
                this.RuleExit ( this.Name );
            }
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as RuleWrapper );
        }

        public Boolean Equals ( RuleWrapper other )
        {
            return other != null &&
                     this.Name == other.Name &&
                    EqualityComparer<Action<String, String[]>>.Default.Equals ( this.RuleMatched, other.RuleMatched ) &&
                    EqualityComparer<Action<String>>.Default.Equals ( this.RuleExit, other.RuleExit ) &&
                    EqualityComparer<Action<String>>.Default.Equals ( this.RuleEnter, other.RuleEnter );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = 593797347;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<String, String[]>>.Default.GetHashCode ( this.RuleMatched );
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<String>>.Default.GetHashCode ( this.RuleExit );
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<String>>.Default.GetHashCode ( this.RuleEnter );
            return hashCode;
        }

        public static Boolean operator == ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => EqualityComparer<RuleWrapper>.Default.Equals ( wrapper1, wrapper2 );

        public static Boolean operator != ( RuleWrapper wrapper1, RuleWrapper wrapper2 ) => !( wrapper1 == wrapper2 );

        #endregion Generated Code
    }
}
