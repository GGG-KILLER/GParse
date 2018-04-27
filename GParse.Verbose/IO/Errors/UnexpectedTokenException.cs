using System;

namespace GParse.Verbose.IO.Errors
{
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException ( String message ) : base ( message )
        {
        }
    }
}
