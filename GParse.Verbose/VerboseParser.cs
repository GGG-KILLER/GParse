using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.Dbug;
using GParse.Verbose.Matchers;

namespace GParse.Verbose
{
    public delegate ASTNode NodeFactory ( String Name, Queue<String> ContentStack, Queue<ASTNode> NodeStack );

    public abstract class VerboseParser
    {
        #region Logging

        public enum LogCategory : Int32
        {
            MatcherParse = 1 << 0,
            RuleGet = 1 << 1,
            RuleSet = 1 << 2,
            RawRuleGet = 1 << 3,
            RuleStart = 1 << 4,
            RuleMatch = 1 << 5,
            RuleEnd = 1 << 6
        }

        public Action<LogCategory, Object> LogDelegate;

        private void Log ( LogCategory category, Object value ) => this.LogDelegate?.Invoke ( category, value );

        #endregion Logging

        private readonly Dictionary<String, BaseMatcher> Rules = new Dictionary<String, BaseMatcher> ( );
        private readonly Dictionary<String, NodeFactory> Factories = new Dictionary<String, NodeFactory> ( );
        private readonly List<String> IgnoredRuleResults = new List<String> ( );
        private (String Name, BaseMatcher Matcher) Root;
        protected Boolean Debug;

        protected VerboseParser ( )
        {
            this.Setup ( );
        }

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
                    if ( pattern[i + 1] == '-' && i + 2 != lastIdx )
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

            this.Log ( LogCategory.MatcherParse, $"Generated {matcher} for {pattern}" );
            return this.Debug ? MatcherDebug.GetDebug ( matcher ) : matcher;
        }

        /// <summary>
        /// Creates a matcher based on the filter passed to it
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        protected BaseMatcher GetMatcher ( Func<Char, Boolean> Filter )
        {
            BaseMatcher matcher = Match.ByFilter ( Filter );
            this.Log ( LogCategory.MatcherParse, $"Generated {matcher} for {Filter}" );
            return this.Debug ? MatcherDebug.GetDebug ( matcher ) : matcher;
        }

        /// <summary>
        /// This function should set up the parser by calling the
        /// <see cref="Rule(String, BaseMatcher, Boolean)" />,
        /// <see cref="Factory(String, NodeFactory)" /> and
        /// <see cref="SetRootRule(String)" /> functions
        /// </summary>
        public abstract void Setup ( );

        /// <summary>
        /// Retrieves a registered rule
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name )
        {
            this.Log ( LogCategory.RuleGet, $"Placeholder for {Name} returned" );
            return new RulePlaceholder ( Name, this );
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
            // Ignore handling
            if ( ShouldIngore )
                this.IgnoredRuleResults.Add ( Name );
            // Rule saving
            this.Rules[Name] = Matcher.As ( Name, this.RuleEnter, this.RuleMatch, this.RuleExit );
            if ( this.Debug )
                this.Rules[Name] = MatcherDebug.GetDebug ( this.Rules[Name] );

            // Return a placeholder
            this.Log ( LogCategory.RuleSet, $"Registered {Name} as {this.Rules[Name]} with ignoring set to: {ShouldIngore}" );
            return new RulePlaceholder ( Name, this );
        }

        /// <summary>
        /// Returns the actual rule's matcher
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public BaseMatcher RawRule ( String Name )
        {
            this.Log ( LogCategory.RawRuleGet, $"Raw rule for {Name} retrieved" );
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

        private static ASTNode Passtrhough ( String a, Queue<String> b, Queue<ASTNode> queue ) => queue.Last ( );

        /// <summary>
        /// Defines a passthrough factory (that basically returns
        /// the first dequeued node)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected NodeFactory PassthroughFactory ( String Name ) => this.Factories[Name] = Passtrhough;

        /// <summary>
        /// Sets the root matcher
        /// </summary>
        /// <param name="Name"></param>
        protected void SetRootRule ( String Name ) => this.Root = (Name, this.Rule ( Name ));

        #endregion Parser Setup

        #region Actual Parsing

        #region State Stack

        public struct ParserState
        {
            public Queue<String> ContentQueue;
            public Queue<ASTNode> NodeQueue;
        }

        private readonly Stack<ParserState> StateStack = new Stack<ParserState> ( );

        private void PushState ( ) => this.StateStack.Push ( new ParserState
        {
            ContentQueue = new Queue<String> ( this.StateStack.Count > 0
                ? ( IEnumerable<String> ) this.StateStack.Peek ( ).ContentQueue
                : new String[0] ),
            NodeQueue = new Queue<ASTNode> ( this.StateStack.Count > 0
                ? ( IEnumerable<ASTNode> ) this.StateStack.Peek ( ).NodeQueue
                : new ASTNode[0] )
        } );

        private void EnqueueContent ( String content )
            => this.StateStack.Peek ( ).ContentQueue.Enqueue ( content ?? throw new ArgumentNullException ( nameof ( content ) ) );

        private void EnqueueNode ( ASTNode node )
            => this.StateStack.Peek ( ).NodeQueue.Enqueue ( node ?? throw new ArgumentNullException ( nameof ( node ) ) );

        private ParserState PopState ( )
        {
            ParserState state = this.StateStack.Pop ( );

            // Adjust previous states to account for dequeueing
            // from their queues
            if ( this.StateStack.Count > 0 )
            {
                ParserState curr = this.StateStack.Peek ( );

                while ( curr.ContentQueue.Count > state.ContentQueue.Count )
                    curr.ContentQueue.Dequeue ( );
                while ( curr.NodeQueue.Count > state.NodeQueue.Count )
                    curr.NodeQueue.Dequeue ( );
            }

            return state;
        }

        #endregion State Stack

        #region Rule Events

        public event Action<String, ParserState> RuleExectionStarted;

        public event Action<String, ParserState, ParserState> RuleExectionEnded;

        public event Action<String, String[], ParserState> RuleExecutionMatched;

        private void RuleEnter ( String Name )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
            {
                return;
            }

            ParserState prev = this.StateStack.Peek ( );
            this.Log ( LogCategory.RuleStart, $"Rule {Name} started." );
            this.Log ( LogCategory.RuleStart, $"PrevState->ContentQueue = [{String.Join ( ", ", prev.ContentQueue )}]" );
            this.PushState ( );
            this.RuleExectionStarted?.Invoke ( Name, prev );
        }

        private void RuleExit ( String Name )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
                return;

            NodeFactory fact = this.Factory ( Name );
            ParserState state = this.StateStack.Peek ( );
            // Enqueue node if there's a factory for this rule
            if ( fact != null )
            {
                ASTNode node = fact ( Name, state.ContentQueue, state.NodeQueue );
                this.PopState ( );
                this.EnqueueNode ( node );
            }
            // otherwise enqueue the content that the rule has found
            else
            {
                this.PopState ( );
                ParserState curr = this.StateStack.Peek ( );
                var delta = new List<String> ( curr.ContentQueue.Count - state.ContentQueue.Count );

                // Update changes on parent
                while ( curr.ContentQueue.Count < state.ContentQueue.Count )
                    curr.ContentQueue.Enqueue ( state.ContentQueue.Dequeue ( ) );
            }

            this.RuleExectionEnded?.Invoke ( Name, state, this.StateStack.Peek ( ) );
        }

        private void RuleMatch ( String Name, String[] Contents )
        {
            if ( this.IgnoredRuleResults.Contains ( Name ) )
                return;
            if ( Contents == null )
                throw new ParseException ( SourceLocation.Min, $"Failed to match rule {Name}." );
            foreach ( var content in Contents )
                this.EnqueueContent ( content );
            this.RuleExecutionMatched?.Invoke ( Name, Contents, this.StateStack.Peek ( ) );
        }

        #endregion Rule Events

        public ASTNode Parse ( String value )
        {
            var reader = new SourceCodeReader ( value );
            BaseMatcher root = this.Root.Matcher;
            ParserState rootState = default;
            try
            {
                if ( !root.IsMatch ( reader, out var _ ) )
                    throw new ParseException ( reader.Location, "Invalid expression." );
                this.PushState ( );
                root.Match ( reader );
                rootState = this.PopState ( );
                if ( rootState.NodeQueue.Count > 1 )
                    throw new ParseException ( reader.Location, $"There's more than one node left in the queue." );
                return rootState.NodeQueue.Dequeue ( );
            }
            catch ( ParseException ex )
            {
                // Exceptions will come without their location set
                // since there's no access to it internally
                ex.Location = reader.Location;
                throw;
            }
        }

        #endregion Actual Parsing

        #region Debug

        public void PrintTree ( )
        {
            if ( this.Debug )
                MatcherDebug.PrintMatcherTree ( this.Root.Matcher );
        }

        public void PrintRules ( )
        {
            if ( this.Debug )
                foreach ( KeyValuePair<String, BaseMatcher> rule in this.Rules )
                    Console.WriteLine ( MatcherDebug.GetRule ( rule.Value ) );
        }

        #endregion Debug
    }
}
