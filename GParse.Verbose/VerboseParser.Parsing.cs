using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.AST;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;
using GParse.Verbose.AST;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Parsing;
using GParse.Verbose.Utilities;
using GParse.Verbose.Visitors;

namespace GParse.Verbose
{
    public abstract partial class VerboseParser : IMatcherTreeVisitor<MatchResult>
    {
        protected SourceCodeReader Reader;
        protected readonly MaximumMatchLengthCalculator LengthCalculator;
        protected Dictionary<String, String> SaveMemory;

        /// <summary>
        /// Whether <see cref="NegatedMatcher"/> should match the maximum length string that it would match or a single char
        /// </summary>
        protected Boolean MaxLengthNegateds;

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
            this.SaveMemory = new Dictionary<String, String> ( );
            MatchResult result = this.RawRule ( this.RootName ).Accept ( this );

            if ( !result.Success )
                throw result.Error;
            if ( result.Nodes.Length < 1 )
                throw new FatalParseException ( this.Reader.Location, "Not enough nodes returned by root rule." );
            if ( result.Nodes.Length > 1 )
                throw new FatalParseException ( this.Reader.Location, "Too many nodes returned by root rule." );
            return result.Nodes[0];
        }

        public MatchResult Visit ( SequentialMatcher SequentialMatcher )
        {
            var stringList = new List<String> ( );
            var nodeList = new List<ASTNode> ( );

            this.Reader.Save ( );
            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
            {
                MatchResult result = matcher.Accept ( this );
                if ( !result.Success )
                {
                    this.Reader.Load ( );
                    return new MatchResult ( new MatcherFailureException ( this.Reader.Location, SequentialMatcher, "Failed to match all patterns.", result.Error ) );
                }

                stringList.AddRange ( result.Strings );
                nodeList.AddRange ( result.Nodes );
            }

            this.Reader.DiscardSave ( );
            return new MatchResult ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        public MatchResult Visit ( AlternatedMatcher AlternatedMatcher )
        {
            var result = new MatchResult ( new MatcherFailureException ( this.Reader.Location, AlternatedMatcher, "Empty AlternatedMatcher" ) );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
            {
                result = matcher.Accept ( this );
                if ( result.Success )
                    return result;
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, AlternatedMatcher, "Failed to match any of the alternatives", result.Error ) );
        }

        public MatchResult Visit ( CharMatcher charMatcher )
        {
            if ( this.Reader.HasContent && this.Reader.IsNext ( charMatcher.Filter ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { charMatcher.StringFilter } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, charMatcher, $"Expected '{StringUtilities.GetCharacterRepresentation ( charMatcher.Filter )}' but got '{StringUtilities.GetCharacterRepresentation ( ( Char ) this.Reader.Peek ( ) )}'" ) );
        }

        public MatchResult Visit ( RangeMatcher RangeMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( this.Reader.HasContent && RangeMatcher.Range.ValueIn ( peek ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }

            var start = StringUtilities.GetCharacterRepresentation ( ( Char ) ( RangeMatcher.Range.Start ) );
            var end = StringUtilities.GetCharacterRepresentation ( ( Char ) ( RangeMatcher.Range.End ) );
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, RangeMatcher, $"Expected character inside range ['{start}', '{end}'] but got '{StringUtilities.GetCharacterRepresentation ( peek )}'" ) );
        }

        public MatchResult Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( this.Reader.HasContent && filterFuncMatcher.Filter ( peek ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, filterFuncMatcher, $"Character '{StringUtilities.GetCharacterRepresentation ( peek )}' did not pass the filter function {filterFuncMatcher.FullFilterName}" ) );
        }

        public MatchResult Visit ( CharListMatcher CharListMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( this.Reader.HasContent && Array.IndexOf ( CharListMatcher.Whitelist, peek ) != -1 )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, CharListMatcher, $"Expected character in whitelist ['{String.Join ( "', '", Array.ConvertAll ( CharListMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) )}'] but got '{StringUtilities.GetCharacterRepresentation ( peek )}'" ) );
        }

        public MatchResult Visit ( RulePlaceholder rulePlaceholder ) => this.RawRule ( rulePlaceholder.Name ).Accept ( this );

        public MatchResult Visit ( StringMatcher stringMatcher )
        {
            if ( this.Reader.HasContent && this.Reader.IsNext ( stringMatcher.StringFilter ) )
            {
                this.Reader.Advance ( stringMatcher.StringFilter.Length );
                return new MatchResult ( new[] { stringMatcher.StringFilter } );
            }
            return new MatchResult ( new MatcherFailureException ( this.Reader.Location, stringMatcher, $"Expected string '{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}' but got '{StringUtilities.GetStringRepresentation ( this.Reader.PeekString ( stringMatcher.StringFilter.Length ) ?? "null (not enough characters until EOF)" )}'" ) );
        }

        public MatchResult Visit ( IgnoreMatcher ignoreMatcher )
        {
            MatchResult res = ignoreMatcher.PatternMatcher.Accept ( this );
            return res.Success ? new MatchResult ( res.Nodes ) : res;
        }

        public MatchResult Visit ( JoinMatcher joinMatcher )
        {
            MatchResult res = joinMatcher.PatternMatcher.Accept ( this );
            return res.Success
                ? new MatchResult ( res.Nodes, new[] { String.Join ( "", res.Strings ) } )
                : res;
        }

        public MatchResult Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var maxlen           = this.MaxLengthNegateds ? this.LengthCalculator.Calculate ( negatedMatcher ) : 1;
            MatchResult res      = negatedMatcher.PatternMatcher.Accept ( this );

            return res.Success
                ? new MatchResult ( new MatcherFailureException ( start, negatedMatcher, "Matched sequence that wasn't meant to be matched." ) )
                : new MatchResult ( new[] { this.Reader.ReadString ( maxlen ) } );
        }

        public MatchResult Visit ( OptionalMatcher optionalMatcher )
        {
            MatchResult res = optionalMatcher.PatternMatcher.Accept ( this );
            return res.Success ? res : new MatchResult ( Array.Empty<String> ( ) );
        }

        public MatchResult Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var nodeList = new List<ASTNode> ( );
            var stringList = new List<String> ( );
            var mcount = 0;

            for ( mcount = 0; mcount < repeatedMatcher.Range.End; mcount++ )
            {
                MatchResult res = repeatedMatcher.PatternMatcher.Accept ( this );
                if ( !res.Success )
                    break;
                nodeList.AddRange ( res.Nodes );
                stringList.AddRange ( res.Strings );
            }
            if ( mcount < repeatedMatcher.Range.Start )
                return new MatchResult ( new MatcherFailureException ( start, repeatedMatcher, "Failed to match the pattern the minimum amount of times." ) );
            return new MatchResult ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        public MatchResult Visit ( RuleWrapper ruleWrapper )
        {
            this.RuleEnter ( ruleWrapper.Name );
            MatchResult res = ruleWrapper.PatternMatcher.Accept ( this );
            if ( res.Success && this.Factories.ContainsKey ( ruleWrapper.Name ) )
                res = new MatchResult ( new[] { this.Factory ( ruleWrapper.Name ) ( ruleWrapper.Name, res ) } );
            this.RuleMatch ( ruleWrapper.Name, res );
            this.RuleExit ( ruleWrapper.Name );
            return res;
        }

        public MatchResult Visit ( MarkerMatcher markerMatcher )
        {
            MatchResult res = markerMatcher.PatternMatcher.Accept ( this );
            if ( res.Success )
            {
                var nodes = new ASTNode[res.Nodes.Length + 1];
                Array.Copy ( res.Nodes, nodes, res.Nodes.Length );
                nodes[nodes.Length + 1] = new MarkerNode ( res.Strings.Length > 0 ? res.Strings[0] : String.Empty );
                res = new MatchResult ( nodes, res.Strings );
            }
            return res;
        }

        public MatchResult Visit ( EOFMatcher eofMatcher )
        {
            return this.Reader.IsAtEOF
                ? new MatchResult ( Array.Empty<String> ( ) )
                : new MatchResult ( new MatcherFailureException ( this.Reader.Location, eofMatcher, "Expected EOF." ) );
        }

        public MatchResult Visit ( SavingMatcher savingMatcher )
        {
            MatchResult res = savingMatcher.PatternMatcher.Accept ( this );
            if ( res.Success )
                this.SaveMemory[savingMatcher.SaveName] = res.Strings[0];
            return res;
        }

        public MatchResult Visit ( LoadingMatcher loadingMatcher )
        {
            // Ensure content existence
            if ( !this.SaveMemory.ContainsKey ( loadingMatcher.SaveName ) )
                return new MatchResult ( new MatcherFailureException ( this.Reader.Location, loadingMatcher, $"Failed to load saved content from memory slot '{loadingMatcher.SaveName}'" ) );

            // Do normal StringMatcher logic
            var content = this.SaveMemory[loadingMatcher.SaveName];
            if ( !this.Reader.IsNext ( content ) )
                return new MatchResult ( new MatcherFailureException ( this.Reader.Location, loadingMatcher, $"Expected '{content}' but got '{this.Reader.PeekString ( content.Length ) ?? "EOF"}'." ) );

            // Return content on success
            this.Reader.Advance ( content.Length );
            return new MatchResult ( new[] { content } );
        }
    }
}
