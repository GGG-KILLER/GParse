using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.IO;
using GParse.Verbose.AST;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Parsing;
using GParse.Verbose.Utilities;
using GParse.Verbose.Visitors;

namespace GParse.Verbose
{
    public abstract partial class VerboseParser : MatcherTreeVisitor<MatchResult>
    {
        protected SourceCodeReader Reader;
        protected readonly MaximumMatchLengthCalculator LengthCalculator;

        #region Rule Events

        public event Action<String> RuleExectionStarted;

        public event Action<String> RuleExectionEnded;

        public event Action<String, MatchResult> RuleExecutionMatched;

        private void RuleEnter ( String Name )
        {
            this.RuleExectionStarted?.Invoke ( Name );
        }

        private void RuleMatch ( String Name, MatchResult Result )
        {
            this.RuleExecutionMatched?.Invoke ( Name, Result );
        }

        private void RuleExit ( String Name )
        {
            this.RuleExectionEnded?.Invoke ( Name );
        }

        #endregion Rule Events

        public ASTNode Parse ( String value )
        {
            this.Reader = new SourceCodeReader ( value );
            MatchResult result = this.Visit ( this.RawRule ( this.RootName ) );

            if ( !result.Success )
                throw result.Error;
            if ( result.Nodes.Length < 1 )
                throw new FatalParseException ( this.Reader.Location, "Not enough nodes returned by root rule." );
            if ( result.Nodes.Length > 1 )
                throw new FatalParseException ( this.Reader.Location, "Too many nodes returned by root rule." );
            return result.Nodes[0];
        }

        public override MatchResult Visit ( AllMatcher allMatcher )
        {
            var stringList = new List<String> ( );
            var nodeList = new List<ASTNode> ( );

            this.Reader.Save ( );
            foreach ( BaseMatcher matcher in allMatcher.PatternMatchers )
            {
                MatchResult result = this.Visit ( matcher );
                if ( !result.Success )
                {
                    this.Reader.LoadSave ( );
                    return new MatchResult ( new MatcherFailureException ( this.Reader.Location, allMatcher, "Failed to match all patterns.", result.Error ) );
                }

                stringList.AddRange ( result.Strings );
                nodeList.AddRange ( result.Nodes );
            }

            this.Reader.DiscardSave ( );
            return new MatchResult ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        public override MatchResult Visit ( AnyMatcher anyMatcher )
        {
            var result = new MatchResult ( new MatcherFailureException ( this.Reader.Location, anyMatcher, "Empty AnyMatcher" ) );
            foreach ( BaseMatcher matcher in anyMatcher.PatternMatchers )
            {
                result = this.Visit ( matcher );
                if ( result.Success )
                    return result;
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, anyMatcher, "Failed to match any of the alternatives", result.Error ) );
        }

        public override MatchResult Visit ( CharMatcher charMatcher )
        {
            if ( !this.Reader.EOF ( ) && this.Reader.IsNext ( charMatcher.Filter ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { charMatcher.StringFilter } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, charMatcher, $"Expected '{StringUtilities.GetCharacterRepresentation ( charMatcher.Filter )}' but got '{StringUtilities.GetCharacterRepresentation ( ( Char ) this.Reader.Peek ( ) )}'" ) );
        }

        public override MatchResult Visit ( CharRangeMatcher charRangeMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( !this.Reader.EOF ( ) && charRangeMatcher.Start < peek && peek < charRangeMatcher.End )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }

            var start = StringUtilities.GetCharacterRepresentation ( charRangeMatcher.Strict
                ? charRangeMatcher.Start
                : ( Char ) ( charRangeMatcher.Start + 1 ) );
            var end = StringUtilities.GetCharacterRepresentation ( charRangeMatcher.Strict
                ? charRangeMatcher.End
                : ( Char ) ( charRangeMatcher.End + 1 ) );
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, charRangeMatcher, $"Expected character inside range ('{start}', '{end}') but got '{StringUtilities.GetCharacterRepresentation ( peek )}'" ) );
        }

        public override MatchResult Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( !this.Reader.EOF ( ) && filterFuncMatcher.Filter ( peek ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, filterFuncMatcher, $"Character '{StringUtilities.GetCharacterRepresentation ( peek )}' did not pass the filter function {filterFuncMatcher.FullFilterName}" ) );
        }

        public override MatchResult Visit ( MultiCharMatcher multiCharMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( !this.Reader.EOF ( ) && Array.IndexOf ( multiCharMatcher.Whitelist, peek ) != -1 )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, multiCharMatcher, $"Expected character in whitelist ['{String.Join ( "', '", Array.ConvertAll ( multiCharMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) )}'] but got '{StringUtilities.GetCharacterRepresentation ( peek )}'" ) );
        }

        public override MatchResult Visit ( RulePlaceholder rulePlaceholder )
        {
            return this.Visit ( this.RawRule ( rulePlaceholder.Name ) );
        }

        public override MatchResult Visit ( StringMatcher stringMatcher )
        {
            if ( !this.Reader.EOF ( ) && this.Reader.IsNext ( stringMatcher.StringFilter ) )
            {
                this.Reader.Advance ( stringMatcher.StringFilter.Length );
                return new MatchResult ( new[] { stringMatcher.StringFilter } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, stringMatcher, $"Expected string '{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}' but got '{StringUtilities.GetStringRepresentation ( this.Reader.PeekString ( stringMatcher.StringFilter.Length ) ?? "null (not enough characters until EOF)" )}'" ) );
        }

        public override MatchResult Visit ( IgnoreMatcher ignoreMatcher )
        {
            MatchResult res = this.Visit ( ignoreMatcher.PatternMatcher );
            return res.Success ? new MatchResult ( res.Nodes ) : res;
        }

        public override MatchResult Visit ( JoinMatcher joinMatcher )
        {
            MatchResult res = this.Visit ( joinMatcher.PatternMatcher );
            return res.Success
                ? new MatchResult ( res.Nodes, new[] { String.Join ( "", res.Strings ) } )
                : res;
        }

        public override MatchResult Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var maxlen           = this.LengthCalculator.Calculate ( negatedMatcher );
            MatchResult res      = this.Visit ( negatedMatcher.PatternMatcher );

            return res.Success
                ? new MatchResult ( new MatcherFailureException ( start, negatedMatcher, "Matched sequence that wasn't meant to be matched." ) )
                : new MatchResult ( new[] { this.Reader.ReadString ( maxlen ) } );
        }

        public override MatchResult Visit ( OptionalMatcher optionalMatcher )
        {
            MatchResult res = this.Visit ( optionalMatcher.PatternMatcher );
            return res.Success ? res : new MatchResult ( Array.Empty<String> ( ) );
        }

        public override MatchResult Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var nodeList = new List<ASTNode> ( );
            var stringList = new List<String> ( );
            var mcount = 0;

            for ( mcount = 0; mcount < repeatedMatcher.Maximum; mcount++ )
            {
                MatchResult res = this.Visit ( repeatedMatcher.PatternMatcher );
                if ( !res.Success )
                    break;
                nodeList.AddRange ( res.Nodes );
                stringList.AddRange ( res.Strings );
            }
            if ( mcount < repeatedMatcher.Minimum )
                return new MatchResult ( new MatcherFailureException ( start, repeatedMatcher, "Failed to match the pattern the minimum amount of times." ) );
            return new MatchResult ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        public override MatchResult Visit ( RuleWrapper ruleWrapper )
        {
            this.RuleEnter ( ruleWrapper.Name );
            MatchResult res = this.Visit ( ruleWrapper.PatternMatcher );
            if ( res.Success && this.Factories.ContainsKey ( ruleWrapper.Name ) )
                res = new MatchResult ( new[] { this.Factory ( ruleWrapper.Name ) ( ruleWrapper.Name, res ) } );
            this.RuleMatch ( ruleWrapper.Name, res );
            this.RuleExit ( ruleWrapper.Name );
            return res;
        }

        public override MatchResult Visit ( MarkerMatcher markerMatcher )
        {
            MatchResult res = this.Visit ( markerMatcher.PatternMatcher );
            if ( res.Success )
            {
                var nodes = new ASTNode[res.Nodes.Length + 1];
                Array.Copy ( res.Nodes, nodes, res.Nodes.Length );
                nodes[nodes.Length + 1] = new MarkerNode ( res.Strings.Length > 0 ? res.Strings[0] : String.Empty );
                res = new MatchResult ( nodes, res.Strings );
            }
            return res;
        }

        public override MatchResult Visit ( EOFMatcher eofMatcher )
        {
            return this.Reader.EOF ( )
                ? new MatchResult ( Array.Empty<String> ( ) )
                : new MatchResult ( new MatcherFailureException ( this.Reader.Location, eofMatcher, "Expected EOF." ) );
        }
    }
}
