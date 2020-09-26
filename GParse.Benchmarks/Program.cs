using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace GParse.Benchmarks
{
    class Program
    {
        static void Main ( String[] args ) =>
            BenchmarkSwitcher.FromAssembly ( typeof ( Program ).Assembly )
                .Run (
                    args
#if DEBUG
                    , new DebugInProcessConfig ( )
#endif
                );
    }
}
