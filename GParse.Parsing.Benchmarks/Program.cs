using System;
using System.Diagnostics;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace GParse.Parsing.Benchmarks
{
    internal class Program
    {
        private static void Main ( )
        {
            BenchmarkRunner.Run<OverflowCheckersBenchmark> ( );
            //BenchmarkRunner.Run<TokenManagerBenchmarks> ( );
        }
    }
}
