using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace GParse.Parsing.Benchmarks
{
    [ClrJob, CoreJob ( )]
    [RPlotExporter, RankColumn, HtmlExporter, MarkdownExporter]
    public class OverflowCheckersBenchmark
    {
        [Params ( 43054U, 1051313684U )]
        public UInt32 LeftHandSide;

        [Params ( 11821U, 414902732U )]
        public UInt32 RightHandSide;

        [Benchmark]
        public Boolean WillOverflowUnchecked ( )
        {
            UInt32 res = unchecked(this.LeftHandSide * this.RightHandSide);
            return res % this.LeftHandSide != 0 || res % this.RightHandSide != 0;
        }

        [Benchmark]
        public Boolean WillOverflowChecked ( )
        {
            try
            {
                UInt32 res = checked(this.LeftHandSide * this.RightHandSide);
                return true;
            }
            catch ( OverflowException )
            {
                return false;
            }
        }
    }
}