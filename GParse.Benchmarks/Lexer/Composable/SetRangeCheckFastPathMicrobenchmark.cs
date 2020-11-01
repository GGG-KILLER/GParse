using System;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Benchmarks.Lexer.Composable
{
    [DisassemblyDiagnoser, MemoryDiagnoser]
    [SimpleJob ( RuntimeMoniker.Net461 )]
    [SimpleJob ( RuntimeMoniker.Net48 )]
    [SimpleJob ( RuntimeMoniker.NetCoreApp21 )]
    [SimpleJob ( RuntimeMoniker.NetCoreApp31 )]
    [SimpleJob ( RuntimeMoniker.NetCoreApp50 )]
    public class SetRangeCheckFastPathMicrobenchmark
    {
        public ImmutableArray<Char> Ranges => CharUtils.FlattenRanges ( new[] { new Range<Char> ( 'a', 'z' ) } );

        [Params ( 'g', 'a', '9' )]
        public Char Value { get; set; }

        [Benchmark ( Baseline = true )]
        public Boolean BinarySearch ( ) =>
            CharUtils.IsInRanges ( this.Ranges, this.Value );

        [Benchmark]
        public Boolean Raw ( )
        {
            ImmutableArray<Char> ranges = this.Ranges;
            return CharUtils.IsInRange ( ranges[0], this.Value, ranges[1] );
        }
    }
}
