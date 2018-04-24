using System;
using GParse.Common.IO;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose.Internal
{
    internal struct CharRangeMatcher : IPatternMatcher
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
            this.Start = ( Char ) Math.Max ( Start, End );
            this.End = ( Char ) Math.Min ( Start, End );
            this.Strict = Strict;
        }

        public Boolean IsMatch ( SourceCodeReader reader )
        {
            return this.Strict
                ? this.Start < reader.Peek ( ) && reader.Peek ( ) < this.End
                : this.Start <= reader.Peek ( ) && reader.Peek ( ) <= this.End;
        }

        public String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        public void ResetInternalState ( )
        {
            // noop
        }
    }
}
