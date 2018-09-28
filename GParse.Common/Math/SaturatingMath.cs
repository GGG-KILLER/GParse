using System;

namespace GParse.Common.Math
{
	public class SaturatingMath
	{
		#region Int32

		#region Overflow/Underflow checks

		public static Boolean WillAdditionOverflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 &&  rhs > 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillAdditionUnderflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs < 0 && lhs <= min - rhs /* ∧ rhs < 0 */;
		}

		public static Boolean WillSubtractionOverflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs > 0 && lhs >= max + rhs /* ∧ rhs < 0 */;
		}

		public static Boolean WillSubtractionUnderflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs < 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillMultiplicationOverflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max / rhs;
		}

		public static Boolean WillMultiplicationUnderflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min / rhs;
		}

		public static Boolean WillDivisionOverflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max * rhs;
		}

		public static Boolean WillDivisionUnderflow ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min * rhs;
		}

		#endregion Overflow/Underflow checks

		#region Math operations

		public static Int32 Add ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillAdditionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs + rhs;
		}

		public static Int32 Subtract ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( WillSubtractionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs - rhs;
		}

		public static Int32 Multiply ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillMultiplicationUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs * rhs;
		}

		public static Int32 Divide ( in Int32 lhs, in Int32 rhs, in Int32 min, in Int32 max )
		{
			if ( WillDivisionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs / rhs;
		}

		#endregion Math operations

		#endregion Int32

		#region Int64

		#region Overflow/Underflow checks

		public static Boolean WillAdditionOverflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 &&  rhs > 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillAdditionUnderflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs < 0 && lhs <= min - rhs /* ∧ rhs < 0 */;
		}

		public static Boolean WillSubtractionOverflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs > 0 && lhs >= max + rhs /* ∧ rhs < 0 */;
		}

		public static Boolean WillSubtractionUnderflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs < 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillMultiplicationOverflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max / rhs;
		}

		public static Boolean WillMultiplicationUnderflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min / rhs;
		}

		public static Boolean WillDivisionOverflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max * rhs;
		}

		public static Boolean WillDivisionUnderflow ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min * rhs;
		}

		#endregion Overflow/Underflow checks

		#region Math operations

		public static Int64 Add ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillAdditionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs + rhs;
		}

		public static Int64 Subtract ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( WillSubtractionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs - rhs;
		}

		public static Int64 Multiply ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillMultiplicationUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs * rhs;
		}

		public static Int64 Divide ( in Int64 lhs, in Int64 rhs, in Int64 min, in Int64 max )
		{
			if ( WillDivisionOverflow ( lhs, rhs, min, max ) )
				return max;
			else if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs / rhs;
		}

		#endregion Math operations

		#endregion Int64

		#region UInt32

		#region Overflow/Underflow check

		public static Boolean WillAdditionOverflow ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillSubtractionUnderflow ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillMultiplicationOverflow ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max / rhs;
		}

		public static Boolean WillDivisionUnderflow ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min * rhs;
		}

		#endregion Overflow/Underflow check

		#region Math operations

		public static UInt32 Add ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
				return max;
			else
				return lhs + rhs;
		}

		public static UInt32 Subtract ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs - rhs;
		}

		public static UInt32 Multiply ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
				return max;
			else
				return lhs * rhs;
		}

		public static UInt32 Divide ( in UInt32 lhs, in UInt32 rhs, in UInt32 min, in UInt32 max )
		{
			if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs / rhs;
		}

		#endregion Math operations

		#endregion UInt32

		#region UInt64

		#region Overflow/Underflow check

		public static Boolean WillAdditionOverflow ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max - rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillSubtractionUnderflow ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min + rhs /* ∧ rhs > 0 */;
		}

		public static Boolean WillMultiplicationOverflow ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs >= max / rhs;
		}

		public static Boolean WillDivisionUnderflow ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( max < min )
				throw new InvalidOperationException ( "Cannot have a maximum value smaller than the minimum value." );

			return lhs != 0 && rhs != 0 && lhs <= min * rhs;
		}

		#endregion Overflow/Underflow check

		#region Math operations

		public static UInt64 Add ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( WillAdditionOverflow ( lhs, rhs, min, max ) )
				return max;
			else
				return lhs + rhs;
		}

		public static UInt64 Subtract ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( WillSubtractionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs - rhs;
		}

		public static UInt64 Multiply ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( WillMultiplicationOverflow ( lhs, rhs, min, max ) )
				return max;
			else
				return lhs * rhs;
		}

		public static UInt64 Divide ( in UInt64 lhs, in UInt64 rhs, in UInt64 min, in UInt64 max )
		{
			if ( WillDivisionUnderflow ( lhs, rhs, min, max ) )
				return min;
			else
				return lhs / rhs;
		}

		#endregion Math operations

		#endregion UInt64

	}
}
