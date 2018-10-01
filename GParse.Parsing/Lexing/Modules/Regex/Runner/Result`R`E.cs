using System;

namespace GParse.Parsing.Lexing.Modules.Regex.Runner
{
    public readonly struct Result<R, E>
    {
        public readonly Boolean Success;
        public readonly R Value;
        public readonly E Error;

        public Result ( R result )
        {
            this.Success = true;
            this.Value = result;
            this.Error = default;
        }

        public Result ( E error )
        {
            this.Success = false;
            this.Value = default;
            this.Error = error;
        }
    }
}
