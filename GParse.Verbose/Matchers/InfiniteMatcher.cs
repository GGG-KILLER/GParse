using System;
using System.Collections.Generic;
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
            while ( this.PatternMatcher.IsMatch ( reader, out var tmpLength, offset + length ) )
                length += tmpLength;
            return length != 0;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            if ( this.IsMatch ( reader, out var _, 0 ) )
            {
                var res = new List<String> ( );
                while ( this.IsMatch ( reader, out var _, 0 ) )
                    res.AddRange ( this.PatternMatcher.Match ( reader ) );
                return res.ToArray ( );
            }
            return null;
        }
    }
}
