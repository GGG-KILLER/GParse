using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Verbose.Lexing;
using GParse.Verbose.Lexing.Compiler;
using GParse.Verbose.Optimization;
using GParse.Verbose.Parsing;

namespace GParse.Verbose
{
    /// <summary>
    /// A lexer class that can be created through expressions (use
    /// either this or <see cref="VerboseParser" />. DO NOT USE BOTH.)
    /// </summary>
    public abstract partial class VerboseLexer
    {
        internal readonly Dictionary<String, RuleDefinition> Rules = new Dictionary<String, RuleDefinition> ( );
        internal readonly Dictionary<String, Func<SourceCodeReader, VerboseLexer, Token>> CompiledRules = new Dictionary<String, Func<SourceCodeReader, VerboseLexer, Token>> ( );
        private readonly ExpressionParser Parser = new ExpressionParser ( );

        protected VerboseLexer ( )
        {
            this.Setup ( );
            this.Predictor = new LexRulePredictor ( this.Rules.Keys.ToArray ( ) );
        }

        protected abstract void Setup ( );

        /// <summary>
        /// Registers a tokenizing rule
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <param name="converter"></param>
        protected void Rule ( String name, String expression, TokenType type = TokenType.Other, Func<String, Object> converter = null )
        {
            if ( String.IsNullOrEmpty ( name ) )
                throw new ArgumentException ( "Name of the rule cannot be null or empty.", nameof ( name ) );
            if ( this.Rules.ContainsKey ( name ) )
                throw new InvalidOperationException ( "Rule name already exists." );

            this.Rules[name] = new RuleDefinition ( name, this.Parser.Parse ( expression ), type, converter );
        }

        /// <summary>
        /// Optimizes tokenizing tree
        /// </summary>
        protected void Optimize ( )
        {
            var optimizer = new MatchTreeOptimizer ( );
            var optimized = new Dictionary<String, RuleDefinition> ( );

            foreach ( KeyValuePair<String, RuleDefinition> kv in this.Rules )
                optimized.Add ( kv.Key, new RuleDefinition ( kv.Key, kv.Value.Body.Accept ( optimizer ), kv.Value.Type, kv.Value.Converter ) );

            foreach ( KeyValuePair<String, RuleDefinition> kv in optimized )
                this.Rules.Add ( kv.Key, kv.Value );
        }

        protected void Compile ( params String[] excludearr )
        {
            var compiler = new LexTreeCompiler ( this );
            var exclude = new HashSet<String> ( excludearr );
            foreach ( KeyValuePair<String, RuleDefinition> kv in this.Rules )
            {
                if ( exclude.Contains ( kv.Key ) )
                    continue;
                this.CompiledRules[kv.Key] = compiler.Compile ( kv.Value, this.CreateToken );
            }
        }
    }
}
