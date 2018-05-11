using System;
using GUtils.Timing;

namespace GParse.CLI
{
    public class LoggerTimingArea : TimingArea
    {
        public LoggerTimingArea ( String name, TimingArea parent = null ) : base ( name, parent )
        {
        }

        public override void Log ( Object Message, Boolean indentIn = true )
        {
            Logger.WriteLine ( $"[{( ( LoggerTimingArea ) this._root )._stopwatch.Elapsed}]{this._indent}{( indentIn ? "\t" : "" )}{Message}" );
        }
    }
}
