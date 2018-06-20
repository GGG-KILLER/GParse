﻿using System;
using System.Collections.Generic;

namespace GParse.Verbose.Matchers
{
    public abstract class MatcherWrapper : BaseMatcher, IEquatable<MatcherWrapper>
    {
        internal readonly BaseMatcher PatternMatcher;

        protected MatcherWrapper ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as MatcherWrapper );
        }

        public Boolean Equals ( MatcherWrapper other )
        {
            return other != null &&
                    EqualityComparer<BaseMatcher>.Default.Equals ( this.PatternMatcher, other.PatternMatcher );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -1683483781;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseMatcher>.Default.GetHashCode ( this.PatternMatcher );
            return hashCode;
        }

        public static Boolean operator == ( MatcherWrapper wrapper1, MatcherWrapper wrapper2 ) => EqualityComparer<MatcherWrapper>.Default.Equals ( wrapper1, wrapper2 );

        public static Boolean operator != ( MatcherWrapper wrapper1, MatcherWrapper wrapper2 ) => !( wrapper1 == wrapper2 );

        #endregion Generated Code
    }
}
