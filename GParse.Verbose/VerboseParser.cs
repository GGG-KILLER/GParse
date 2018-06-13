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
using GParse.Verbose.Parser;

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
        private String RootName;
        protected Boolean Debug;

        protected VerboseParser ( )
        {
            this.ExpressionParser = new MatchExpressionParser ( this );
            this.Setup ( );

            if ( this.RootName == null )
                throw new InvalidOperationException ( "Can't initialize a parser without a root rule." );
        }

        #region Parser Setup

        #region Matcher Parsing

        /// <summary>
        /// Creates a Matcher based on the pattern passed to it.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected BaseMatcher ParseMatcher ( String pattern )
        {
            BaseMatcher matcher = this.ExpressionParser.Parse ( pattern );
            return this.Debug ? MatcherDebug.GetDebug ( matcher ) : matcher;
        }

        /// <summary>
        /// Creates a matcher based on the filter passed to it
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        protected BaseMatcher ParseMatcher ( Func<Char, Boolean> Filter )
        {
            BaseMatcher matcher = Match.ByFilter ( Filter );
            return this.Debug ? MatcherDebug.GetDebug ( matcher ) : matcher;
        }

        #endregion Matcher Parsing

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
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            return new RulePlaceholder ( Name, this );
        }

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
            return Rule ( Name, this.ParseMatcher ( Expression ) );
        }

        /// <summary>
        /// Registers a rule
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

            // Rule saving
            this.Rules[Name] = new RuleWrapper ( Matcher, Name, this.RuleEnter, this.RuleMatch, this.RuleExit );
            if ( this.Debug )
                this.Rules[Name] = MatcherDebug.GetDebug ( this.Rules[Name] );

            // Return a placeholder
            return new RulePlaceholder ( Name, this );
        }

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

        /// <summary>
        /// Sets the root matcher
        /// </summary>
        /// <param name="Name"></param>
        protected void SetRootRule ( String Name )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );

            this.RootName = Name;
        }

        /// <summary>
        /// Sets the root matcher
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Matcher"></param>
        /// <returns></returns>
        protected BaseMatcher SetRootRule ( String Name, BaseMatcher Matcher )
        {
            if ( String.IsNullOrWhiteSpace ( Name ) )
                throw new ArgumentException ( "Rule name can't be whitespaces, null or empty", nameof ( Name ) );
            if ( Matcher == null )
                throw new ArgumentNullException ( nameof ( Matcher ) );

            BaseMatcher rule = this.Rule ( Name, Matcher );
            this.SetRootRule ( Name );
            return rule;
        }

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

        public Stack<String> ContentStack;
        public Stack<ASTNode> NodeStack;

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
                throw new ParseException ( SourceLocation.Min, $"Failed to match rule {Name}." );
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
                    MatcherDebug.Logger.WriteLine ( MatcherDebug.GetRule ( rule.Value ) );
        }

        #endregion Debug
    }
}
