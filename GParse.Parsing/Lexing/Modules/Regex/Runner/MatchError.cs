using System;
using GParse.Common;

namespace GParse.Parsing.Lexing.Modules.Regex.Runner
{
    public struct MatchError
    {
        public readonly SourceLocation Location;
        public readonly String Message;

        public MatchError ( SourceLocation loc, String msg )
        {
            this.Location = loc;
            this.Message = msg;
        }
    }
}
