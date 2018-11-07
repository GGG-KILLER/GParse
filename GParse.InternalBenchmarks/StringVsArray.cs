using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace GParse.Benchmarks
{
    [ClrJob, CoreJob ( isBaseline: true )]
    public class StringVsArray
    {
        [Params ( 250, 500 * 1024, 25 * 1024 * 1024 )]
        public Int32 Length;

        public String Content;

        [IterationSetup]
        public void IterationSetup ( ) => this.Content = new String ( 'x', this.Length );

        [Benchmark ( Description = "Indexing a string" )]
        public Char[] IndexingString ( )
        {
            var arr = new Char[this.Length];
            for ( var i = 0; i < this.Length; i++ )
                arr[i] = this.Content[i];
            return arr;
        }

        [Benchmark ( Description = "Indexing a array" )]
        public Char[] IndexingArray ( )
        {
            var arr1 = this.Content.ToCharArray ( );
            var arr2 = new Char[this.Length];
            for ( var i = 0; i < this.Length; i++ )
                arr2[i] = arr1[i];
            return arr2;
        }
    }
}
