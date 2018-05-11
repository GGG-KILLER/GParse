using System;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class RulePlaceholder : BaseMatcher
    {
        internal readonly VerboseParser Parser;
        internal readonly String Name;

        public RulePlaceholder ( String Name, VerboseParser Parser )
        {
            this.Name = Name;
            this.Parser = Parser;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            return this.Parser.RawRule ( this.Name ).IsMatch ( reader, out length, offset );
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.Parser.RawRule ( this.Name ).Match ( reader );
        }

        public override void ResetInternalState ( )
        {
            this.Parser.RawRule ( this.Name ).ResetInternalState ( );
        }
    }
}
