//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Jobs;

//namespace GParse.Benchmarks.Lexer.Composable.OptimizationAlgorithms
//{
//    using GParse.Lexing.Composable;

//    [DisassemblyDiagnoser, MemoryDiagnoser]
//    [SimpleJob ( RuntimeMoniker.Net461 )]
//    [SimpleJob ( RuntimeMoniker.Net48 )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp21 )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp31 )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp50 )]
//    public class MergeRangesMicrobenchmark
//    {
//        private List<CharacterRange> _ranges;
//        private List<CharacterRange> _iterationRanges;

//        [Params ( "a-e|g-k", "a-d|e-h|j-m|n-t", "a-b|c-d|e-f|g-h|i-j|k-l|m-n|o-p|r-s|t-u|v-w" )]
//        public String Ranges
//        {
//            set
//            {
//                this._ranges = value.Split ( '|' )
//                                    .Select ( s => new CharacterRange ( s[0], s[2] ) )
//                                    .ToList ( );
//                this._iterationRanges = new List<CharacterRange> ( this._ranges );
//            }
//        }

//        [Benchmark ( Baseline = true )]
//        public void InPlace ( )
//        {
//            this._iterationRanges.Clear ( );
//            this._iterationRanges.AddRange ( this._ranges );
//            OptimizationAlgorithms.MergeRanges ( this._iterationRanges );
//        }

//        [Benchmark]
//        public void OutPlace ( )
//        {
//            this._iterationRanges.Clear ( );
//            this._iterationRanges.AddRange ( this._ranges );
//            _ = OptimizationAlgorithms.MergeRangesOutPlace ( this._iterationRanges );
//        }
//    }
//}