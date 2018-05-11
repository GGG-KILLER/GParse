using System;
using System.Text;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class InfiniteMatcher : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;

        public InfiniteMatcher ( BaseMatcher matcher )
        {
            this.PatternMatcher = matcher ?? throw new ArgumentNullException ( nameof ( matcher ) );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset )
        {
            if ( reader.EOF ( ) )
            {
                length = 0;
                return false;
            }

            length = 0;
            while ( this.PatternMatcher.IsMatch ( reader, out length, length ) )
                /* do nothing since it'll be updating itself here ↑*/
                ;
            return length != 0;
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader, out var _, 0 ) )
            {
                var sb = new StringBuilder ( );
                while ( this.IsMatch ( reader, out var _, 0 ) )
                    sb.Append ( this.PatternMatcher.Match ( reader ) );
                return sb.ToString ( );
            }
            return null;
        }
    }
}
