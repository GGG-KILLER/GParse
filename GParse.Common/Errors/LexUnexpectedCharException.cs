using System;

namespace GParse.Common.Errors
{
    public class UnexpectedCharException : LocationBasedException
    {
        public readonly Char UnexpectedChar;

        public UnexpectedCharException ( SourceLocation location, Char ch ) : base ( location, $"Unexpected character: '{ch}'" )
        {
            this.UnexpectedChar = ch;
        }
    }
}
