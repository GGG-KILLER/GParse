using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;

namespace GParse.Verbose.Exceptions
{
    public class MatchExpressionException : Exception
    {
        public readonly SourceLocation Location;

        public MatchExpressionException ( SourceLocation location, String message ) : base ( message )
        {
            this.Location = location;
        }

        public MatchExpressionException ( SourceLocation location, String message, Exception innerException ) : base ( message, innerException )
        {
            this.Location = location;
        }
    }
}
