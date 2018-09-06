using System;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Fluent.Exceptions

{
    public class InvalidExpressionException : LocationBasedException
    {
        public InvalidExpressionException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public InvalidExpressionException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
