using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using GParse.Common.IO;

namespace GParse.Parsing.Benchmarks
{
    [ClrJob, CoreJob ( isBaseline: true )]
    [RPlotExporter, RankColumn, HtmlExporter, MarkdownExporter]
    public class TokenManagerBenchmarks
    {
        private const String Code01 = @"var1=var2;";
        private const String Code02 = @"if(var1!=var2)var1=var2;";
        private const String Code03 = @"if(var1&var2!=var3&var4){var5=!((var2|var3)&&(var4|var1));}else{var5=(var2&var3)||(var4&var1));}";

        private TokenManager TokenManager;
        private SourceCodeReader CodeReader;

        private static Boolean IsIdentifierChar ( Char ch ) => Char.IsLetterOrDigit ( ch ) || ch == '_';

        [Params ( Code01, Code02, Code03 )]
        public String Code = Code03;
        public Queue<Token> DummyTokenQueue;

        [GlobalSetup]
        public void Setup ( )
        {
            this.TokenManager = new TokenManager ( )
                .AddToken ( "If", "if", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Else", "else", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Return", "return", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "True", "true", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "False", "false", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Identifier", "var1", TokenType.Identifier, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Identifier", "var2", TokenType.Identifier, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Identifier", "var3", TokenType.Identifier, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Identifier", "var4", TokenType.Identifier, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "Identifier", "var5", TokenType.Identifier, ch => !IsIdentifierChar ( ch ) )
                .AddToken ( "LCurly", "{", TokenType.LCurly )
                .AddToken ( "RCurly", "}", TokenType.RCurly )
                .AddToken ( "LParen", "(", TokenType.LParen )
                .AddToken ( "RParen", ")", TokenType.RParen )
                .AddToken ( "&", "&", TokenType.Operator )
                .AddToken ( "And", "&&", TokenType.Operator )
                .AddToken ( "BOr", "|", TokenType.Operator )
                .AddToken ( "Or", "||", TokenType.Operator )
                .AddToken ( "Not", "!", TokenType.Operator )
                .AddToken ( "NotEq", "!=", TokenType.Operator )
                .AddToken ( "Assign", "=", TokenType.Operator )
                .AddToken ( "Eq", "==", TokenType.Operator )
                .AddToken ( "Semicolon", ";", TokenType.Punctuation );
            this.CodeReader = new SourceCodeReader ( this.Code );
            this.DummyTokenQueue = new Queue<Token> ( 50 );
        }

        [GlobalCleanup]
        public void Cleanup ( )
        {
            this.DummyTokenQueue.Clear ( );
        }

        [Benchmark]
        public void TokenizeAll ( )
        {
            while ( !this.CodeReader.EOF ( ) )
                this.DummyTokenQueue.Enqueue ( this.TokenManager.ReadToken ( this.CodeReader ) );
        }
    }
}
