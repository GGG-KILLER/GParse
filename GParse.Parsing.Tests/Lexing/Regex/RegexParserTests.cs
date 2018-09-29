using System;
using GParse.Common.Math;
using GParse.Parsing.Lexing.Modules.Regex;
using GParse.Parsing.Lexing.Modules.Regex.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace GParse.Parsing.Tests.Lexing.Regex
{
    [TestClass]
    public class RegexParserTests
    {
        [TestMethod]
        public void Works ( )
        {
            var tests = new (String, Node)[]
            {
                ( "abc", new Sequence ( new Node[]
                {
                    new Literal ( 'a' ),
                    new Literal ( 'b' ),
                    new Literal ( 'c' )
                } ) ),
                ( "[abc]", new Alternation ( new Node[]
                {
                    new Literal ( 'a' ),
                    new Literal ( 'b' ),
                    new Literal ( 'c' )
                }, false ) ),
                ( "[^abc]", new Alternation ( new Node[]
                {
                    new Literal ( 'a' ),
                    new Literal ( 'b' ),
                    new Literal ( 'c' )
                }, true ) ),
                ( "a|b|c", new Alternation ( new Node[]
                {
                    new Literal ( 'a' ),
                    new Literal ( 'b' ),
                    new Literal ( 'c' )
                }, false ) ),
                // Lua comments (broken since we don't have backtracking)
                ( @"--\[(=*)\[(?:[^\]]*)\]\1\]", new Sequence ( new Node[]
                {
                    new Literal ( '-' ),
                    new Literal ( '-' ),
                    new Literal ( '[' ),
                    new Capture ( 1,
                    new Repetition ( new Range<UInt32> ( 0, UInt32.MaxValue ),
                        new Literal ( '=' ) ) ),
                    new Literal ( '[' ),
                    new Repetition ( new Range<UInt32> ( 0, UInt32.MaxValue ),
                        new Alternation ( new Node[]
                        {
                            new Literal ( ']' )
                        }, true ) ),
                    new Literal ( ']' ),
                    new CaptureReference ( 1 ),
                    new Literal ( ']' )
                } ) ),
                ( @" *?", new Repetition ( new Range<UInt32> ( 0, UInt32.MaxValue ),
                    new Literal ( ' ' ) ) { IsLazy = true } ),
                ( @"\d+", new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ),
                    new Range ( new Range<Char> ( '0', '9' ) ) ) ),
                ( @"\p{Digit}+", new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ),
                    new Range ( new Range<Char> ( '0', '9' ) ) ) ),
                ( @"0[xX](?:[[:xdigit:]]+(?:\.[[:xdigit:]]+)?(?:[pP][[:digit:]]+)?|\.[[:xdigit:]]+(?:[pP][[:digit:]]+)?)", new Sequence ( new Node[]
                {
                    new Literal ( '0' ),
                    new Alternation ( new Node[]
                    {
                        new Literal ( 'x' ),
                        new Literal ( 'X' )
                    }, false ),
                    new Alternation ( new Node[]
                    {
                        new Sequence ( new Node[]
                        {
                            new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ), new Alternation ( new Node[]
                            {
                                new Range ( new Range<Char> ( 'A', 'F' ) ),
                                new Range ( new Range<Char> ( 'a', 'f' ) ),
                                new Range ( new Range<Char> ( '0', '9' ) )
                            }, false ) ),
                            new Sequence ( new Node[]
                            {
                                new Literal ( '.' ),
                                new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ), new Alternation ( new Node[]
                                {
                                    new Range ( new Range<Char> ( 'A', 'F' ) ),
                                    new Range ( new Range<Char> ( 'a', 'f' ) ),
                                    new Range ( new Range<Char> ( '0', '9' ) )
                                }, false ) ),
                            } ) { IsLazy = true },
                            new Sequence ( new Node[]
                            {
                                new Alternation ( new Node[]
                                {
                                    new Literal ( 'p' ),
                                    new Literal ( 'P' )
                                }, false ),
                                new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ), new Range ( new Range<Char> ( '0' , '9' ) ) )
                            } ) { IsLazy = true }
                        } ),
                        new Sequence ( new Node[]
                        {
                            new Literal ( '.' ),
                            new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ), new Alternation ( new Node[]
                            {
                                new Range ( new Range<Char> ( 'A', 'F' ) ),
                                new Range ( new Range<Char> ( 'a', 'f' ) ),
                                new Range ( new Range<Char> ( '0', '9' ) )
                            }, false ) ),
                            new Sequence ( new Node[]
                            {
                                new Alternation ( new Node[]
                                {
                                    new Literal ( 'p' ),
                                    new Literal ( 'P' )
                                }, false ),
                                new Repetition ( new Range<UInt32> ( 1, UInt32.MaxValue ), new Range ( new Range<Char> ( '0' , '9' ) ) )
                            } ) { IsLazy = true }
                        } ),
                    }, false )
                } ) )
            };

            foreach ( (var expression, Node expected) in tests )
            {
                Logger.LogMessage ( "Testing {0}...", expression );
                Node parsed = new RegexParser ( expression ).Parse ( );
                Logger.LogMessage ( "\tExpected: {0}", expected );
                Logger.LogMessage ( "\tGotten:   {0}", parsed );
                Assert.AreEqual ( expected, parsed );
            }
        }
    }
}
