using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Verbose.Dbug
{
    internal class DummyLogger : ILogger
    {
        public void Indent ( String sname )
        {
            return;
        }

        public void Outdent ( )
        {
            return;
        }

        public void WriteLine ( Object line )
        {
            return;
        }
    }
}
