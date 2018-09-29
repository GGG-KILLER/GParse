using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Parsing.Lexing.Modules.Regex
{
    public class InvalidRegexException : LocationBasedException
    {
        public InvalidRegexException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public InvalidRegexException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
