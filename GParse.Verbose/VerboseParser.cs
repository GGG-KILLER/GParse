using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Dbug;
using GParse.Verbose.Matchers;

namespace GParse.Verbose
{
    public delegate ASTNode NodeFactory ( String Name, Stack<String> ContentStack, Stack<ASTNode> NodeStack );

    public abstract class VerboseParser
    {
        private readonly Dictionary<String, BaseMatcher> Rules = new Dictionary<String, BaseMatcher> ( );
        private readonly Dictionary<String, NodeFactory> Factories = new Dictionary<String, NodeFactory> ( );
        private readonly List<String> IgnoredRuleResults = new List<String> ( );
        private (String Name, BaseMatcher Matcher) Root;
        private CompiledRule CompiledRoot;
        protected Boolean Debug;

        protected VerboseParser ( )
        {
            this.Setup ( );
            //this.Compile ( );
        }

        #region Compilation

        #region Compilation Assets

        protected delegate Boolean CompiledRuleIsMatch ( SourceCodeReader reader, Int32 offset );

        protected delegate String CompiledRuleMatch ( SourceCodeReader reader );

        protected struct CompiledRule
        {
            public CompiledRuleIsMatch IsMatch;
            public CompiledRuleMatch Match;
        }

        #endregion Compilation Assets

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
        private void Compile ( )
        {
            if ( this.Root == default ( (String, BaseMatcher) ) )
                throw new Exception ( "Root rule not set." );
            ParameterExpression reader = Expression.Parameter ( typeof ( SourceCodeReader ), "reader" );
            ParameterExpression offset = Expression.Parameter ( typeof ( Int32 ), "offset" );
            this.CompiledRoot = new CompiledRule
            {
                IsMatch = Expression.Lambda<CompiledRuleIsMatch> (
                    ReduceExpr ( this.Root.Matcher.IsMatchExpression ( reader, offset ) ), reader, offset ).Compile ( ),
                Match = Expression.Lambda<CompiledRuleMatch> (
                    ReduceExpr ( this.Root.Matcher.MatchExpression ( reader ) ), reader ).Compile ( )
            };
        }

        #endregion Compilation

        #region Parser Setup

        /// <summary>
        /// Creates a Matcher based on the pattern passed to it.
        /// </summary>
        /// <param name="pattern">
        /// Accepted pattern types: "[a-zA-Z]", "string",
        /// "[abcd]", "[abcde-h]"
        /// </param>
        /// <returns></returns>
        protected BaseMatcher GetMatcher ( String pattern )
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

            return this.Debug ? MatcherDebug.GetDebug ( matcher ) : matcher;
        }

        /// <summary>
        /// Creates a matcher based on the filter passed to it
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        protected BaseMatcher GetMatcher ( Func<Char, Boolean> Filter )
            => this.Debug ? MatcherDebug.GetDebug ( Match.ByFilter ( Filter ) ) : Match.ByFilter ( Filter );

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
        protected BaseMatcher Rule ( String Name )
        {
            return this.Rules[Name];
        }

        /// <summary>
        /// Registers a rule
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <param name="ShouldIngore"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, BaseMatcher Matcher, Boolean ShouldIngore = false )
        {
            if ( ShouldIngore )
                this.IgnoredRuleResults.Add ( Name );
            this.Rules[Name] =  Matcher.As ( Name, this.RuleEnter, this.RuleMatch, this.RuleExit );
            if ( this.Debug )
                this.Rules[Name] = MatcherDebug.GetDebug ( this.Rules[Name] );

            return this.Rules[Name];
        }

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

        #region Actual Parsing

        #region State Stack

        private struct ParserState
        {
            public Stack<String> ContentStack;
            public Stack<ASTNode> NodeStack;
        }

        private Stack<ParserState> StateStack = new Stack<ParserState> ( );

        private void PushState ( ) => this.StateStack.Push ( new ParserState
        {
            ContentStack = new Stack<String> ( this.StateStack.Count > 0
                ? ( IEnumerable<String> ) this.StateStack.Peek ( ).ContentStack
                : new String[0] ),
            NodeStack = new Stack<ASTNode> ( this.StateStack.Count > 0
                ? ( IEnumerable<ASTNode> ) this.StateStack.Peek ( ).NodeStack
                : new ASTNode[0] )
        } );

        private void PushContent ( String content )
            => this.StateStack.Peek ( ).ContentStack.Push ( content );

        private void PushNode ( ASTNode node )
            => this.StateStack.Peek ( ).NodeStack.Push ( node );

        private ParserState PopState ( )
        {
            ParserState state = this.StateStack.Pop ( );

            // Adjust previous states to account for popping from
            // their stacks
            if ( this.StateStack.Count > 0 )
            {
                ParserState curr = this.StateStack.Peek ( );

                while ( curr.ContentStack.Count > state.ContentStack.Count )
                    curr.ContentStack.Pop ( );
                while ( curr.NodeStack.Count > state.NodeStack.Count )
                    curr.NodeStack.Pop ( );
            }

            return state;
        }

        #endregion State Stack

        #region Rule Events

        public event Action<String> RuleExectionStarted;

        public event Action<String> RuleExectionEnded;

        public event Action<String, String> RuleExecutionMatched;

        private void RuleEnter ( String Name )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
                return;

            this.PushState ( );
            this.RuleExectionStarted?.Invoke ( Name );
        }

        private void RuleExit ( String Name )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
                return;

            NodeFactory fact = this.Factory ( Name );
            ParserState state = this.StateStack.Peek ( );
            // Assert this node can be transformed
            if ( fact != null )
            {
                ASTNode node = fact ( Name, state.ContentStack, state.NodeStack );
                this.PopState ( );
                this.PushNode ( node );
            }
            else
            {
                this.PopState ( );
                ParserState curr = this.StateStack.Peek ( );
                var delta = new List<String> ( curr.ContentStack.Count - state.ContentStack.Count );

                // Get changes
                while ( curr.ContentStack.Count < state.ContentStack.Count )
                    delta.Add ( state.ContentStack.Pop ( ) );
                delta.Reverse ( );

                // Add changes in the same order
                foreach ( var content in delta )
                    curr.ContentStack.Push ( content );
            }

            this.RuleExectionEnded?.Invoke ( Name );
        }

        private void RuleMatch ( String Name, String Content )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
                return;
            Console.WriteLine ( $"{Name}\t\t\"{Content ?? "null"}\"" );
            if ( Content == null )
                throw new ParseException ( SourceLocation.Min, $"Failed to match rule {Name}." );
            Console.WriteLine ( "boop" );
            this.PushContent ( Content );
            this.RuleExecutionMatched?.Invoke ( Name, Content );
        }

        #endregion Rule Events

        public ASTNode Parse ( String value )
        {
            var reader = new SourceCodeReader ( value );
            //CompiledRule root = this.CompiledRoot;
            BaseMatcher root = this.Root.Matcher;
            ParserState rootState = default;
            try
            {
                this.PushState ( );
                root.Match ( reader );
                rootState = this.PopState ( );
                return rootState.NodeStack.Pop ( );
            }
            catch ( ParseException ex )
            {
                throw new ParseException ( reader.Location, ex.OriginalMessage );
            }
            //catch ( Exception ex )
            //{
            //    throw new ParseException ( reader.Location, ex.Message );
            //}
            finally
            {
                if ( rootState.NodeStack.Count > 0 )
                    throw new ParseException ( reader.Location, $"There are still nodes left in the node stack." );
            }
        }

        #endregion Actual Parsing

        #region Debug
#if DEBUG
        public void PrintTree ( )
        {
            MatcherDebug.PrintMatcherTree ( this.Root.Matcher );
        }
#endif
        #endregion Debug
    }
}
