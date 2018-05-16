using System;

namespace GParse.Common.Errors
{
    public class LexUnexpectedCharException : Exception
    {
        public readonly Char UnexpectedChar;

        public LexUnexpectedCharException ( String message, Char ch ) : base ( message )
        {
            this.UnexpectedChar = ch;
        }
    }
}
