using System;
using System.Diagnostics;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace GParse.Benchmarks
{
    internal class Program
    {
        private static void Main ( String[] args )
        {
            BenchmarkSwitcher
                .FromAssembly ( typeof ( Program ).Assembly )
                .Run ( args );
        }
    }
}
