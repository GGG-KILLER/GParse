using System;
using GParse.Common.Errors;

namespace GParse.Fluent.Lexing
{
    public struct LexResult
    {
        public readonly Boolean Success;
        public readonly String Match;
        public readonly LexingException Error;

        public LexResult ( String match )
        {
            this.Success = true;
            this.Match = match;
            this.Error = null;
        }

        public LexResult ( LexingException error )
        {
            this.Success = false;
            this.Match = default;
            this.Error = error;
        }
    }
}
