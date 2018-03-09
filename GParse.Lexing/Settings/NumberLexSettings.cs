using System;

namespace GParse.Lexing.Settings
{
	public struct IntegerLexSettings : IEquatable<IntegerLexSettings>
	{
		[Flags]
		public enum NumberType
		{
			Decimal = 1 << 0,
			Binary = 1 << 1,
			Octal = 1 << 2,
			Hexadecimal = 1 << 3
		}

		/// <summary>
		/// The prefix used in binary numbers
		/// </summary>
		public String BinaryPrefix;

		/// <summary>
		/// The prefix used in decimal numbers
		/// </summary>
		public String DecimalPrefix;

		/// <summary>
		/// The prefix used in hexadecimal numbers
		/// </summary>
		public String HexadecimalPrefix;

		/// <summary>
		/// The prefix used in octal numbers
		/// </summary>
		public String OctalPrefix;

		/// <summary>
		/// The default type of number
		/// </summary>
		public NumberType DefaultType;

		#region Generated Code

		public override Boolean Equals ( Object obj )
		{
			return obj is IntegerLexSettings && this.Equals ( ( IntegerLexSettings ) obj );
		}

		public Boolean Equals ( IntegerLexSettings other )
		{
			return this.BinaryPrefix == other.BinaryPrefix
					 && this.DecimalPrefix == other.DecimalPrefix
					 && this.HexadecimalPrefix == other.HexadecimalPrefix
					 && this.OctalPrefix == other.OctalPrefix;
		}

		public override Int32 GetHashCode ( )
		{
			var hashCode = -1458715052;
			hashCode = ( hashCode * -1521134295 ) + base.GetHashCode ( );
			hashCode = ( hashCode * -1521134295 ) + this.BinaryPrefix.GetHashCode ( );
			hashCode = ( hashCode * -1521134295 ) + this.DecimalPrefix.GetHashCode ( );
			hashCode = ( hashCode * -1521134295 ) + this.HexadecimalPrefix.GetHashCode ( );
			return ( hashCode * -1521134295 ) + this.OctalPrefix.GetHashCode ( );
		}

		public static Boolean operator == ( IntegerLexSettings settings1, IntegerLexSettings settings2 )
		{
			return settings1.Equals ( settings2 );
		}

		public static Boolean operator != ( IntegerLexSettings settings1, IntegerLexSettings settings2 )
		{
			return !( settings1 == settings2 );
		}

		#endregion Generated Code
	}
}
