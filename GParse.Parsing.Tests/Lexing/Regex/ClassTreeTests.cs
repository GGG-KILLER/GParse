using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.IO;
using GParse.Parsing.Lexing.Modules.Regex.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Parsing.Tests.Lexing.Regex
{
    [TestClass]
    public class ClassTreeTests
    {
        [TestMethod]
        public void Works ( )
        {
            var tree = new RegexClassTree ( );
            tree.AddClass ( @"\d", new Literal ( 'd' ) );
            tree.AddClass ( @"[:digit:]", new Literal ( 'd' ) );
            tree.AddClass ( @"\p{Digit}", new Literal ( 'd' ) );

            Assert.AreEqual ( 'd', ( tree.FindClass (
                new SourceCodeReader ( @"\d" )
            ).Item2 as Literal )?.Value );
            Assert.AreEqual ( 'd', ( tree.FindClass (
                new SourceCodeReader ( @"[:digit:]" )
            ).Item2 as Literal )?.Value );
            Assert.AreEqual ( 'd', ( tree.FindClass (
                new SourceCodeReader ( @"\p{Digit}" )
            ).Item2 as Literal )?.Value );
            Assert.IsNull ( tree.FindClass (
                new SourceCodeReader ( @"\D" )
            ).Item2 );
            Assert.IsNull ( tree.FindClass (
                new SourceCodeReader ( @"\p{" )
            ).Item2 );
        }
    }
}
