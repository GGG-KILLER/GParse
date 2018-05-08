using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common;
using GParse.Common.IO;
using GParse.Verbose.Matchers;

namespace GParse.Verbose
{
    public delegate ASTNode NodeFactory ( String Name, String Content, Stack<Object> NodeStack );

    public abstract class VerboseParser
    {
        #region Compilation Assets

        protected delegate String CompiledRuleIsMatch ( SourceCodeReader reader, Int32 offset );

        protected delegate String CompiledRuleMatch ( SourceCodeReader reader, Action<String, String> MatchedListener );

        protected struct CompiledRule
        {
            public CompiledRuleIsMatch IsMatch;
            public CompiledRuleMatch Match;
        }

        #endregion Compilation Assets

        protected Dictionary<String, BaseMatcher> Rules = new Dictionary<String, BaseMatcher> ( );
        protected Dictionary<String, CompiledRule> CompiledRules = new Dictionary<String, CompiledRule> ( );
        protected Dictionary<String, NodeFactory> Factories = new Dictionary<String, NodeFactory> ( );
        protected (String Name, BaseMatcher Matcher) Root;

        protected VerboseParser ( )
        {
            this.Setup ( );
        }

        #region Compilation

        /// <summary>
        /// Reduces an expression as much as possible
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static Expression ReduceExpr ( Expression expr )
        {
            while ( expr.CanReduce )
                expr = expr.Reduce ( );
            return expr;
        }

        /// <summary>
        /// You should call this only once.
        /// </summary>
        public void Compile ( )
        {
            ParameterExpression reader = Expression.Parameter ( typeof ( SourceCodeReader ), "reader" );
            ParameterExpression offset = Expression.Parameter ( typeof ( Int32 ), "offset" );
            ParameterExpression listener = Expression.Parameter ( typeof ( Action<String, String> ), "matchedListener" );
            foreach ( KeyValuePair<String, BaseMatcher> kv in this.Rules )
            {
                this.CompiledRules[kv.Key] = new CompiledRule
                {
                    // Compile as reduced as possible String Match
                    // ( SourceCodeReader, Action<String,String> )
                    Match = Expression.Lambda<CompiledRuleMatch> (
                        ReduceExpr ( kv.Value.MatchExpression ( reader, listener ) ), new[] { reader, listener } ).Compile ( ),
                    // Compile as reduced as possible Boolean
                    // IsMatch ( SourceCodeReader, Int32 )
                    IsMatch = Expression.Lambda<CompiledRuleIsMatch> (
                        ReduceExpr ( kv.Value.IsMatchExpression ( reader, offset ) ), new[] { reader, offset } ).Compile ( )
                };
            }
            if ( this.Root == default ( ( String, BaseMatcher ) ) )
                throw new Exception ( "Root rule not set." );
        }

        #endregion Compilation

        #region Parser Setup

        /// <summary>
        /// Creates a Matcher based on the pattern passed to it.
        /// Accepted pattern types: "[a-zA-Z]", "string",
        /// "[abcd]", "[abcde-h]"
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected static BaseMatcher GetMatcher ( String pattern )
        {
            BaseMatcher matcher = null;
            var lastIdx = pattern.Length - 1;

            if ( pattern[0] == '[' && pattern[lastIdx] == ']' )
            {
                // Range
                for ( var i = 1; i < lastIdx; i++ )
                {
                    BaseMatcher match;
                    var start = pattern[i];
                    if ( pattern[i + 1] == '-' )
                    {
                        var end = pattern[i + 2];
                        i += 2;

                        match = Match.CharRange ( start, end );
                    }
                    else
                        match = Match.Char ( start );
                    matcher = matcher?.Or ( match ) ?? match;
                }
            }
            else
            {
                matcher = pattern.Length == 1
                    ? Match.Char ( pattern[0] )
                    : Match.String ( pattern );
            }

            return matcher;
        }

        /// <summary>
        /// This function should set up the parser by calling the
        /// <see cref="Rule(String, BaseMatcher)" /> and
        /// <see cref="Factory(String, NodeFactory)" /> functions
        /// </summary>
        public abstract void Setup ( );

        /// <summary>
        /// Retrieves a registered rule
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name ) => this.Rules[Name];

        /// <summary>
        /// Registers a rule
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, BaseMatcher Matcher ) => this.Rules[Name] = Matcher;

        /// <summary>
        /// Retrieves a registered node factory
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected NodeFactory Factory ( String Name ) => this.Factories[Name];

        /// <summary>
        /// Registers a node factory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        protected NodeFactory Factory ( String Name, NodeFactory Factory ) => this.Factories[Name] = Factory;

        /// <summary>
        /// Sets the root matcher
        /// </summary>
        /// <param name="Name"></param>
        protected void SetRootRule ( String Name ) => this.Root = (Name, this.Rule ( Name ));

        #endregion Parser Setup
    }
}
