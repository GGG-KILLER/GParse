using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;
using GParse.Lexing;
using GParse.Parsing.Verbose.Abstractions;

namespace GParse.Parsing.Verbose
{
    public delegate ASTNode NodeFactory ( Rule source, String input );

    public class Rule
    {
        public String Name { get; private set; }
        public IPatternMatcher Matcher { get; private set; }
        private readonly NodeFactory NodeFactory;

        public Rule ( String name, IPatternMatcher patternMatcher )
        {
            this.Name = name;
            this.Matcher = patternMatcher;
        }
    }
}
