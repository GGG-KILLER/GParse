using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Verbose.AST;
using GParse.Verbose.Dbug;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Optimization;
using GParse.Verbose.Parser;
using GParse.Verbose.MatcherTreeVisitors;

namespace GParse.Verbose
{
    public delegate ASTNode NodeFactory ( String Name, Stack<String> ContentStack, Stack<ASTNode> NodeStack );

    public enum OperatorAssociativity
    {
        Left,
        Right,
        DontCare
    }

    public abstract class VerboseParser
    {
        private readonly Dictionary<String, BaseMatcher> Rules = new Dictionary<String, BaseMatcher> ( );
        private readonly Dictionary<String, NodeFactory> Factories = new Dictionary<String, NodeFactory> ( );
        private readonly MatchExpressionParser ExpressionParser;
        protected Boolean Debug;
        internal String RootName;

        protected VerboseParser ( )
        {
            this.ExpressionParser = new MatchExpressionParser ( this );
            this.Setup ( );

            if ( this.RootName == null )
                throw new InvalidOperationException ( "Can't initialize a parser without a root rule." );
        }

        #region Parser Setup

        /// <summary>
        /// This function should set up the parser by calling the
        /// <see cref="Rule(String, BaseMatcher, Boolean)" />,
        /// <see cref="Factory(String, NodeFactory)" /> and
        /// <see cref="SetRootRule(String)" /> functions
        /// </summary>
        public abstract void Setup ( );

        /// <summary>
        /// Optimizes all rules in the parser according to the
        /// provided <see cref="TreeOptimizerOptions" />
        /// </summary>
        /// <param name="optimizerOptions"></param>
        public void Optimize ( TreeOptimizerOptions optimizerOptions )
        {
            if ( this.RootName == null )
                throw new InvalidOperationException ( "Optimization can only be done at the end of setup." );
            var optimizer = new MatchTreeOptimizer ( optimizerOptions );
            var optimized = new Dictionary<String, BaseMatcher> ( );

            foreach ( KeyValuePair<String, BaseMatcher> kv in this.Rules )
                optimized[kv.Key] = optimizer.Visit ( kv.Value );

            foreach ( KeyValuePair<String, BaseMatcher> kv in optimized )
                this.Rules[kv.Key] = kv.Value;
        }

        #region Matcher Parsing

        /// <summary>
        /// Creates a Matcher based on the pattern passed to it.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected BaseMatcher ParseMatcher ( String pattern )
        {
            BaseMatcher matcher = this.ExpressionParser.Parse ( pattern );
            return this.Debug ? MatcherDebug.GetDebugTree ( matcher ) : matcher;
        }

        /// <summary>
        /// Creates a matcher based on the filter passed to it
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        protected BaseMatcher ParseMatcher ( Func<Char, Boolean> Filter )
        {
            BaseMatcher matcher = Match.ByFilter ( Filter );
            return this.Debug ? MatcherDebug.GetDebugTree ( matcher ) : matcher;
        }

        #endregion Matcher Parsing

        #region Rule Registering

        /// <summary>
        /// Registers a rule from a string expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Expression"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, String Expression )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( String.IsNullOrWhiteSpace ( Expression ) )
                throw new ArgumentException ( "Expression can't be whitespaces, null or empty", nameof ( Expression ) );

            return this.Rule ( Name, this.ParseMatcher ( Expression ) );
        }

        /// <summary>
        /// Registers a rule from a string expression along with
        /// it's factory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Expression"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, String Expression, NodeFactory Factory )
        {
            this.Factory ( Name, Factory );
            return this.Rule ( Name, Expression );
        }

        /// <summary>
        /// Registers a rule from a matcher
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, BaseMatcher Matcher )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( Matcher == null )
                throw new ArgumentNullException ( nameof ( Matcher ) );

            this.Rules[Name] = new RuleWrapper ( Matcher, Name, this.RuleEnter, this.RuleMatch, this.RuleExit );
            if ( this.Debug )
                this.Rules[Name] = MatcherDebug.GetDebugTree ( this.Rules[Name] );

            return new RulePlaceholder ( Name, this );
        }

        /// <summary>
        /// Registers a rule from a matcher along with it's factory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        protected BaseMatcher Rule ( String Name, BaseMatcher Matcher, NodeFactory Factory )
        {
            this.Factory ( Name, Factory );
            return this.Rule ( Name, Matcher );
        }

        /// <summary>
        /// Sets the root rule from an expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Expression"></param>
        /// <returns></returns>
        protected BaseMatcher RootRule ( String Name, String Expression )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( String.IsNullOrWhiteSpace ( Expression ) )
                throw new ArgumentException ( "Rule expression can't be whitespaces, null or empty", nameof ( Expression ) );

            this.RootName = Name;
            return this.Rule ( Name, Expression );
        }

        /// <summary>
        /// Sets the root rule from an expression along with it's factory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Expression"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        protected BaseMatcher RootRule ( String Name, String Expression, NodeFactory Factory )
        {
            this.Factory ( Name, Factory );
            return this.RootRule ( Name, Expression );
        }

        /// <summary>
        /// Sets the root matcher
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <returns></returns>
        protected BaseMatcher RootRule ( String Name, BaseMatcher Matcher )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( Matcher == null )
                throw new ArgumentNullException ( nameof ( Matcher ) );

            this.RootName = Name;
            return this.Rule ( Name, Matcher );
        }

        #endregion Rule Registering

        /// <summary>
        /// Returns the actual rule's matcher
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public BaseMatcher RawRule ( String Name )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );

            return this.Rules[Name];
        }

        #region Factory Management

        /// <summary>
        /// Retrieves a registered node factory
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected NodeFactory Factory ( String Name )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Factory name can't be whitespaces, null or empty", nameof ( Name ) );

            return this.Factories[Name];
        }

        /// <summary>
        /// Registers a node factory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        protected NodeFactory Factory ( String Name, NodeFactory Factory )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Factory name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( Factory == null )
                throw new ArgumentNullException ( nameof ( Factory ) );

            return this.Factories[Name] = Factory;
        }

        #endregion Factory Management

        #region Language Creation Helpers

        protected BaseMatcher InfixOperator ( String Name, OperatorAssociativity associativity, String @operator,
            BaseMatcher lhsMatcher, BaseMatcher rhsMatcher, Boolean lhsIsParent = true )
        {
            // Sanity checks
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "Operator can't be whitespaces, null or empty", nameof ( @operator ) );
            if ( lhsMatcher == null )
                throw new ArgumentNullException ( nameof ( lhsMatcher ) );
            if ( rhsMatcher == null )
                throw new ArgumentNullException ( nameof ( rhsMatcher ) );

            // Generate the rules along with parenting if required
            BaseMatcher @internal = this.Rule ( $"{Name}<internal>", lhsMatcher + ParseMatcher ( @operator ) + rhsMatcher );
            BaseMatcher rule = this.Rule ( $"{Name}", lhsIsParent ? @internal | lhsMatcher : @internal );

            // Left associativity is the exception beacuse of right-recursion
            if ( associativity == OperatorAssociativity.Left )
            {
                this.Factory ( $"{Name}<internal>", ( _, ContentStack, NodeStack ) =>
                {
                    ASTNode rhs = NodeStack.Pop ( );
                    ASTNode lhs = NodeStack.Pop ( );
                    var op = ContentStack.Pop ( );
                    if ( rhs is BinaryOperatorExpression binaryOperator && binaryOperator.Operator == op )
                        return new BinaryOperatorExpression ( op,
                            new BinaryOperatorExpression ( op, lhs, binaryOperator.LeftHandSide ),
                            binaryOperator.RightHandSide );
                    else
                        return new BinaryOperatorExpression ( op, lhs, rhs );
                } );
            }
            else
            {
                this.Factory ( $"{Name}<internal>", ( _, ContentStack, NodeStack ) =>
                {
                    ASTNode rhs = NodeStack.Pop ( );
                    ASTNode lhs = NodeStack.Pop ( );
                    var op = ContentStack.Pop ( );
                    return new BinaryOperatorExpression ( op, lhs, rhs );
                } );
            }

            return rule;
        }

        #endregion Language Creation Helpers

        #endregion Parser Setup

        #region Actual Parsing

        private Stack<String> ContentStack;
        private Stack<ASTNode> NodeStack;

        #region Rule Events

        public event Action<String, Stack<String>, Stack<ASTNode>> RuleExectionStarted;

        public event Action<String, Stack<String>, Stack<ASTNode>> RuleExectionEnded;

        public event Action<String, String[], Stack<String>, Stack<ASTNode>> RuleExecutionMatched;

        private void RuleEnter ( String Name )
        {
            this.RuleExectionStarted?.Invoke ( Name, this.ContentStack, this.NodeStack );
        }

        private void RuleMatch ( String Name, String[] Contents )
        {
            if ( Contents == null )
                throw new ArgumentNullException ( nameof ( Contents ) );
            foreach ( var content in Contents.Reverse ( ) )
                this.ContentStack.Push ( content );

            // Stack node if there's a factory for this rule
            if ( this.Factories.ContainsKey ( Name ) )
            {
                NodeFactory fact = this.Factory ( Name );
                ASTNode node = fact ( Name, this.ContentStack, this.NodeStack );
                this.NodeStack.Push ( node );
            }

            this.RuleExecutionMatched?.Invoke ( Name, Contents, this.ContentStack, this.NodeStack );
        }

        private void RuleExit ( String Name )
        {
            this.RuleExectionEnded?.Invoke ( Name, this.ContentStack, this.NodeStack );
        }

        #endregion Rule Events

        internal void MarkerMatcherMatched ( String[] Contents )
        {
            if ( Contents == null )
                throw new ArgumentNullException ( nameof ( Contents ) );
            this.NodeStack.Push ( new MarkerNode ( Contents[0] ) );
        }

        public ASTNode Parse ( String value )
        {
            var reader = new SourceCodeReader ( value );
            try
            {
                BaseMatcher root = this.RawRule ( this.RootName );
                this.ContentStack = new Stack<String> ( );
                this.NodeStack = new Stack<ASTNode> ( );
                root.Match ( reader );

                if ( this.NodeStack.Count > 1 )
                    throw new FatalParseException ( reader.Location, $"There's more than one node left in the Stack." );
                if ( !reader.EOF ( ) )
                    throw new FatalParseException ( reader.Location, $"Unfinished expression. (Left to be parsed: {reader})" );
                return this.NodeStack.Pop ( );
            }
            catch ( FatalParseException ex )
            {
                if ( ex.Location == SourceLocation.Min )
                    ex = new FatalParseException ( reader.Location, ex.OriginalMessage );
                throw;
            }
            catch ( ParseException ex )
            {
                if ( ex.Location == SourceLocation.Min )
                    ex = new ParseException ( reader.Location, ex.OriginalMessage );
                throw;
            }
        }

        #endregion Actual Parsing

        #region Debug

        public void PrintTree ( )
        {
            if ( this.Debug )
                MatcherDebug.PrintMatcherTree ( this.RawRule ( this.RootName ) );
        }

        public void PrintRules ( )
        {
            if ( this.Debug )
                foreach ( KeyValuePair<String, BaseMatcher> rule in this.Rules )
                    MatcherDebug.Logger.WriteLine ( MatcherDebug.GetEBNF ( rule.Value ) );
        }

        public void PrintExpressions ( )
        {
            if ( this.Debug )
            {
                var rebuilder = new ExpressionReconstructor ( );
                foreach ( KeyValuePair<String, BaseMatcher> rule in this.Rules )
                    MatcherDebug.Logger.WriteLine ( $"{rule.Key} = {rebuilder.Visit ( rule.Value ) }" );
            }
        }

        #endregion Debug
    }
}
