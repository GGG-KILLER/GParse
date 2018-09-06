using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Fluent.Exceptions
{
    public class InvalidLexerException : Exception
    {
        public InvalidLexerException ( String message ) : base ( message )
        {
        }

        public InvalidLexerException ( String message, Exception innerException ) : base ( message, innerException )
        {
        }
    }
}
