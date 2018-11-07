﻿using System;
using System.Collections.Generic;
using GParse.Common.Math;
using GParse.Fluent.Abstractions;

namespace GParse.Fluent.Matchers
{
    public sealed class RangeMatcher : BaseMatcher, IEquatable<RangeMatcher>
    {
        public readonly Range<Char> Range;

        /// <summary>
        /// </summary>
        /// <param name="range">The range itself</param>
        public RangeMatcher ( in Range<Char> range )
        {
            this.Range = range;
        }

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        public RangeMatcher ( in Char Start, in Char End ) : this ( new Range<Char> ( Start, End ) )
        {
        }

        public override void Accept ( IMatcherTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( IMatcherTreeVisitor<T> visitor ) => visitor.Visit ( this );

        #region Generated Code

        public override Boolean Equals ( Object obj ) => this.Equals ( obj as RangeMatcher );

        public Boolean Equals ( RangeMatcher other ) => other != null && this.Range.Equals ( other.Range );

        public override Int32 GetHashCode ( )
        {
            var hashCode = -2061379221;
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            return hashCode;
        }

        public static Boolean operator == ( RangeMatcher matcher1, RangeMatcher matcher2 ) => EqualityComparer<RangeMatcher>.Default.Equals ( matcher1, matcher2 );

        public static Boolean operator != ( RangeMatcher matcher1, RangeMatcher matcher2 ) => !( matcher1 == matcher2 );

        #endregion Generated Code
    }
}
