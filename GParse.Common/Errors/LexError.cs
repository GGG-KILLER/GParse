using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Common.Errors
{
	public struct LexError
	{
		public SourceLocation Location;
		public String Message;
	}
}
