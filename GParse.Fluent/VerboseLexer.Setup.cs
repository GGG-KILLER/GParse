using System;
using System.Collections.Generic;
using System.Linq;
using GParse.IO;
using GParse.Lexing;
using GParse.Fluent.Lexing;
using GParse.Fluent.Lexing.Compiler;
using GParse.Fluent.Optimization;
using GParse.Fluent.Parsing;

namespace GParse.Fluent
{
    /// <summary>
    /// A lexer class that can be created through expressions (use
    /// either this or <see cref="FluentParser{NodeT}" />. DO NOT
    /// USE BOTH.)
    /// </summary>
    public abstract partial class FluentLexer<TokenTypeT> where TokenTypeT : Enum
    {
        internal readonly Dictionary<String, RuleDefinition<TokenTypeT>> Rules = new Dictionary<String, RuleDefinition<TokenTypeT>> ( );
        internal readonly Dictionary<String, Func<SourceCodeReader, FluentLexer<TokenTypeT>, Token<TokenTypeT>>> CompiledRules = new Dictionary<String, Func<SourceCodeReader, FluentLexer<TokenTypeT>, Token<TokenTypeT>>> ( );
        private readonly ExpressionParser Parser = new ExpressionParser ( );

        /// <summary>
        /// Initializes this class
        /// </summary>
        protected FluentLexer ( )
        {
            this.Setup ( );
            this.Predictor = new LexRulePredictor ( this.Rules.Keys.ToArray ( ) );
        }

        /// <summary>
        /// Setups the rules for this lexer
        /// </summary>
        protected abstract void Setup ( );

        /// <summary>
        /// Registers a tokenizing rule
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <param name="converter"></param>
        protected void Rule ( String name, String expression, TokenTypeT type, Func<String, Object> converter = null )
        {
            if ( String.IsNullOrEmpty ( name ) )
                throw new ArgumentException ( "Name of the rule cannot be null or empty.", nameof ( name ) );
            if ( this.Rules.ContainsKey ( name ) )
                throw new InvalidOperationException ( "Rule name already exists." );

            this.Rules[name] = new RuleDefinition<TokenTypeT> ( name, this.Parser.Parse ( expression ), type, converter );
        }

        /// <summary>
        /// Optimizes tokenizing tree
        /// </summary>
        protected void Optimize ( )
        {
            var optimizer = new MatchTreeOptimizer ( );
            var optimized = new Dictionary<String, RuleDefinition<TokenTypeT>> ( );

            foreach ( KeyValuePair<String, RuleDefinition<TokenTypeT>> kv in this.Rules )
                optimized.Add ( kv.Key, new RuleDefinition<TokenTypeT> ( kv.Key, kv.Value.Body.Accept ( optimizer ), kv.Value.Type, kv.Value.Converter ) );

            foreach ( KeyValuePair<String, RuleDefinition<TokenTypeT>> kv in optimized )
                this.Rules.Add ( kv.Key, kv.Value );
        }

        /// <summary>
        /// Compiles all rules
        /// </summary>
        /// <param name="excludearr"></param>
        protected void Compile ( params String[] excludearr )
        {
            var compiler = new LexTreeCompiler<TokenTypeT> ( this );
            var exclude = new HashSet<String> ( excludearr );
            foreach ( KeyValuePair<String, RuleDefinition<TokenTypeT>> kv in this.Rules )
            {
                if ( exclude.Contains ( kv.Key ) )
                    continue;
                this.CompiledRules[kv.Key] = compiler.Compile ( kv.Value, this.CreateToken );
            }
        }
    }
}
