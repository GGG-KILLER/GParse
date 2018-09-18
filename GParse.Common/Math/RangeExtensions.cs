﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Common.Math
{
    using System;

    public static class RangeExtensions
    {
        #region IsNeighbourOf

        public static Boolean IsNeighbourOf ( in this Range<Char> lhs, in Range<Char> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<Byte> lhs, in Range<Byte> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<SByte> lhs, in Range<SByte> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<Int32> lhs, in Range<Int32> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<UInt32> lhs, in Range<UInt32> rhs )
        {
            return ( lhs.End - rhs.Start ) == 1
                || ( rhs.Start - lhs.End ) == 1
                || ( lhs.Start - rhs.End ) == 1
                || ( rhs.End - lhs.Start ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<Int64> lhs, in Range<Int64> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<UInt64> lhs, in Range<UInt64> rhs )
        {
            return ( lhs.End - rhs.Start ) == 1
                || ( rhs.Start - lhs.End ) == 1
                || ( lhs.Start - rhs.End ) == 1
                || ( rhs.End - lhs.Start ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<Double> lhs, in Range<Double> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        public static Boolean IsNeighbourOf ( in this Range<Single> lhs, in Range<Single> rhs )
        {
            return Math.Abs ( lhs.End - rhs.Start ) == 1
                || Math.Abs ( lhs.Start - rhs.End ) == 1;
        }

        #endregion IsNeighbourOf
    }
}