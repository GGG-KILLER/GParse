using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Verbose.Exceptions
{
    public class MatchExpressionException : ParseException
    {
        public MatchExpressionException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public MatchExpressionException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
