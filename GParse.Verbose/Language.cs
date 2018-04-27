using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose
{
    public class Language
    {
        public String Name { get; }

        public String BaseRule { get; private set; }

        private readonly Dictionary<String, IPatternMatcher> Rules;
        private readonly Dictionary<String, Func<Language, SourceCodeReader, String, ASTNode>> Transformers;

        public Language ( String Name )
        {
            this.Name = Name;
            this.Rules = new Dictionary<String, IPatternMatcher> ( );
            this.Transformers = new Dictionary<String, Func<Language, SourceCodeReader, String, ASTNode>> ( );
        }

        public Language AddRule ( String name, IPatternMatcher matcher )
        {
            this.Rules.Add ( name, matcher );
            return this;
        }

        public IPatternMatcher GetRule ( String name )
        {
            return this.Rules[name];
        }

        public Language SetBaseRule ( String rule )
        {
            if ( !this.Rules.ContainsKey ( rule ) )
                throw new InvalidOperationException ( "Cannot set a non-existent rule as a base rule." );

            this.BaseRule = rule;
            return this;
        }

        public Language AddTransformer ( String rule, Func<Language, SourceCodeReader, String, ASTNode> func )
        {
            if ( !this.Rules.ContainsKey ( rule ) )
                throw new InvalidOperationException ( "Cannot add a translator for a non-existent rule as a base rule." );

            this.Transformers[rule] = func;
            return this;
        }

        public ASTNode Parse ( String input )
        {
            var reader = new SourceCodeReader ( input );
            if ( !this.Rules[this.BaseRule].IsMatch ( reader ) )
                throw new ParseException ( SourceLocation.Zero, "String could not be parsed." );

            return this.Transformers[this.BaseRule] ( this, reader, this.Rules[this.BaseRule].Match ( reader ) );
        }
    }
}
