using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace GParse.Benchmarks.IO.StringCodeReader
{
    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class FindOffsetCharMicrobenchmark
    {
        private readonly String _code = @"aaa bbb ccc
ddd eee fff
ggg hhh iii
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
aaa aaa aaa
lll mmm nnn";

        public Int32 Position { get; set; }

        public Int32 Length => this._code.Length;

        [Params('e', 'h', 'm')]
        public Char Needle { get; set; }

        [Benchmark(Baseline = true)]
        public Int32 FindOffsetNormal()
        {
            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length)
                return -1;

            var idx = this._code.IndexOf(this.Needle, this.Position);
            return idx == -1 ? idx : idx - this.Position;
        }

        [Benchmark]
        public Int32 FindOffsetSpan()
        {
            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position);
            return span.IndexOf(this.Needle);
        }
    }
}