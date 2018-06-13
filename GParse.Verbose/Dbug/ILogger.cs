using System;
using System.Collections.Generic;
using System.Text;

namespace GParse.Verbose.Dbug
{
    public interface ILogger
    {
        void Indent ( String scopeName );
        void Outdent ( );
        void WriteLine ( Object line );
    }
}
