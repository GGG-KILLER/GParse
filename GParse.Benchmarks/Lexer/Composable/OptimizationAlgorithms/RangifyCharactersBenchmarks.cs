//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Jobs;

//namespace GParse.Benchmarks.Lexer.Composable.OptimizationAlgorithms
//{
//    using GParse.Lexing.Composable;

//    [DisassemblyDiagnoser, MemoryDiagnoser]
//    [SimpleJob ( RuntimeMoniker.Net461, launchCount, warmupCount, iterationCount, invocationCount )]
//    [SimpleJob ( RuntimeMoniker.Net48, launchCount, warmupCount, iterationCount, invocationCount )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp21, launchCount, warmupCount, iterationCount, invocationCount )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp31, launchCount, warmupCount, iterationCount, invocationCount )]
//    [SimpleJob ( RuntimeMoniker.NetCoreApp50, launchCount, warmupCount, iterationCount, invocationCount )]
//    public class RangifyCharactersBenchmarks
//    {
//        private const Int32 launchCount     = -1; // 1;
//        private const Int32 warmupCount     = -1; // 10;
//        private const Int32 iterationCount  = -1; // 25;
//        private const Int32 invocationCount = -1; // 1_000_000;

//        private List<Char> _characters;
//        private List<Char> _iterationCharacters;
//        private readonly List<CharacterRange> _iterationRanges = new List<CharacterRange> ( );

//        [Params ( "abcdefghijklmnopqrstuvwxyz0123456789", "abcdefghijkmopqrstuwxyz023456789", "acegikmoqsuwy02468" )]
//        public String Characters
//        {
//            set
//            {
//                this._characters = value.ToList ( );
//                this._iterationCharacters = value.ToList ( );
//            }
//        }

//        [Benchmark ( Baseline = true )]
//        public void InPlace ( )
//        {
//            this._iterationRanges.Clear ( );
//            this._iterationCharacters.Clear ( );
//            this._iterationCharacters.AddRange ( this._characters );
//            OptimizationAlgorithms.RangifyCharacters ( this._iterationCharacters, this._iterationRanges );
//        }

//        [Benchmark]
//        public void OutPlace ( )
//        {
//            this._iterationRanges.Clear ( );
//            this._iterationCharacters.Clear ( );
//            this._iterationCharacters.AddRange ( this._characters );
//            OptimizationAlgorithms.RangifyCharacters ( this._iterationCharacters, this._iterationRanges, out _, out _ );
//        }
//    }
//}
