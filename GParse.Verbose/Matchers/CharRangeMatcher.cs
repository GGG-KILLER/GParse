using System;
using GParse.Common.Errors;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    public sealed class CharRangeMatcher : BaseMatcher
    {
        internal readonly Boolean Strict;
        internal readonly Char Start;
        internal readonly Char End;

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        /// <param name="Strict">
        /// Whether to use Start &lt; value &lt; End instead of
        /// Start ≤ value ≤ End
        /// </param>
        public CharRangeMatcher ( Char Start, Char End, Boolean Strict )
        {
            this.Start = ( Char ) Math.Min ( Start, End );
            this.End = ( Char ) Math.Max ( Start, End );
            this.Strict = Strict;

            if ( !this.Strict )
            {
                this.Start--;
                this.End++;
            }
        }

        public override Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 )
        {
            var ch = reader.Peek ( offset );
            var res = this.Start < ch && ch < this.End;
            length = res ? 1 : 0;
            return res;
        }

        public override String[] Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader, out var _ )
                ? new[] { reader.ReadString ( 1 ) }
                : throw new ParseException ( reader.Location, $"Character '{( Char ) reader.Peek ( )}' did not fit the interval ('{this.Start}', '{this.End}')." );
        }
    }
}
