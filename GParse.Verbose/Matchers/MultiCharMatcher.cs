using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class MultiCharMatcher : BaseMatcher
    {
        internal readonly Char[] Whitelist;

        public MultiCharMatcher ( params Char[] whitelist )
        {
            this.Whitelist = whitelist;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var ismatch = !reader.EOF ( );
            if ( ismatch )
            {
                var ch = ( Char ) reader.Peek ( );
                ismatch = false;
                // This is better for arrays where the most common
                // characters come first, since it'll take less time.
                for ( var i = 0; i < this.Whitelist.Length && !ismatch; i++ )
                    if ( ch == this.Whitelist[i] )
                        ismatch = true;
            }
            length = ismatch ? 1 : 0;
            return ismatch;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader, out var _ )
                ? new[] { reader.ReadString ( 1 ) }
                : throw new ParseException ( reader.Location, $"Expected any of the following: ['{String.Join ( "', '", this.Whitelist )}'] but got '{( Char ) reader.Peek ( )}'" );
        }
    }
}
