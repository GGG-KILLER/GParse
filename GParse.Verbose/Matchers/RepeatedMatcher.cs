using System;
using System.Text;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class RepeatedMatcher : BaseMatcher
    {
        internal readonly BaseMatcher PatternMatcher;
        internal readonly Int32 Limit;

        public RepeatedMatcher ( BaseMatcher matcher, Int32 limit )
        {
            this.PatternMatcher = matcher;
            this.Limit = limit;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
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
            if ( this.IsMatch ( reader, out var _ ) )
            {
                var sb = new StringBuilder ( );
                for ( var i = 0; i < this.Limit && this.IsMatch ( reader, out var _ ); i++ )
                    sb.Append ( this.PatternMatcher.Match ( reader ) );
                return sb.ToString ( );
            }
            return null;
        }
    }
}
