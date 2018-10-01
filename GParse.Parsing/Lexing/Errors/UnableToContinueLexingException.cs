using System;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Parsing.Lexing.Errors
{
    public class UnableToContinueLexingException : LocationBasedException
    {
        public UnableToContinueLexingException ( SourceLocation location, String message, SourceCodeReader reader ) : base ( location, message )
        {
            this.Reader = reader;
        }

        public UnableToContinueLexingException ( SourceLocation location, String message, SourceCodeReader reader, Exception innerException ) : base ( location, message, innerException )
        {
            this.Reader = reader;
        }

        public SourceCodeReader Reader { get; }
    }
}
