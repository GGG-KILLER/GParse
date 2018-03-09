using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Lexing.Errors
{
	public struct LexError
	{
		public SourceLocation Location;
		public String Message;
	}
}
