using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace GParse.Benchmarks
{
    [ClrJob, CoreJob ( )]
    [RPlotExporter, RankColumn, HtmlExporter, MarkdownExporter]
    public class OverflowChecking
    {
        [Params ( 43054U, 1051313684U )]
        public UInt32 LeftHandSide;

        [Params ( 11821U, 414902732U )]
        public UInt32 RightHandSide;

        [Benchmark]
        public Boolean WillOverflowUnchecked ( )
        {
            var res = unchecked(this.LeftHandSide * this.RightHandSide);
            return res % this.LeftHandSide != 0 || res % this.RightHandSide != 0;
        }

        [Benchmark]
        public Boolean WillOverflowChecked ( )
        {
            try
            {
                var res = checked(this.LeftHandSide * this.RightHandSide);
                return true;
            }
            catch ( OverflowException )
            {
                return false;
            }
        }
    }
}