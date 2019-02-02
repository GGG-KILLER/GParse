using System;
using System.Collections.Generic;
using GParse;
using GParse.Errors;
using GParse.IO;
using GParse.Utilities;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Parsing;
using GParse.Fluent.Visitors;

namespace GParse.Fluent
{
    /// <summary>
    /// Fluent parser
    /// </summary>
    public abstract partial class FluentParser<NodeT> : IMatcherTreeVisitor<MatchResult<NodeT>>
    {
        /// <summary>
        /// Reader
        /// </summary>
        protected SourceCodeReader Reader;

        /// <summary>
        /// Length calculator
        /// </summary>
        protected readonly MaximumMatchLengthCalculator<NodeT> LengthCalculator;

        /// <summary>
        /// Save slots
        /// </summary>
        protected Dictionary<String, String> SaveMemory;

        /// <summary>
        /// Whether <see cref="NegatedMatcher" /> should match the
        /// maximum length string that it would match or a single char
        /// </summary>
        protected Boolean MaxLengthNegateds { get; set; }

        #region Rule Events

        /// <summary>
        /// Callback for rule execution started
        /// </summary>
        public event Action<String> RuleExectionStarted;

        /// <summary>
        /// Callback for rule execution ended
        /// </summary>
        public event Action<String> RuleExectionEnded;

        /// <summary>
        /// Callback for rule execution success
        /// </summary>
        public event Action<String, MatchResult<NodeT>> RuleExecutionMatched;

        private void RuleEnter ( String Name ) => this.RuleExectionStarted?.Invoke ( Name );

        private void RuleMatch ( String Name, MatchResult<NodeT> Result ) => this.RuleExecutionMatched?.Invoke ( Name, Result );

        private void RuleExit ( String Name ) => this.RuleExectionEnded?.Invoke ( Name );

        #endregion Rule Events

        /// <summary>
        /// Parses the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public NodeT Parse ( String value )
        {
            this.Reader = new SourceCodeReader ( value );
            this.SaveMemory = new Dictionary<String, String> ( );
            MatchResult<NodeT> result = this.RawRule ( this.RootName ).Accept ( this );

            if ( !result.Success )
                throw result.Error;
            if ( result.Nodes.Length < 1 )
                throw new FatalParsingException ( this.Reader.Location, "Not enough nodes returned by root rule." );
            if ( result.Nodes.Length > 1 )
                throw new FatalParsingException ( this.Reader.Location, "Too many nodes returned by root rule." );
            return result.Nodes[0];
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="SequentialMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( SequentialMatcher SequentialMatcher )
        {
            var stringList = new List<String> ( );
            var nodeList = new List<NodeT> ( );
            SourceLocation startLocation = this.Reader.Location;

            foreach ( BaseMatcher matcher in SequentialMatcher.PatternMatchers )
            {
                MatchResult<NodeT> result = matcher.Accept ( this );
                if ( !result.Success )
                {
                    this.Reader.Rewind ( startLocation );
                    return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, SequentialMatcher, "Failed to match all patterns.", result.Error ) );
                }

                stringList.AddRange ( result.Strings );
                nodeList.AddRange ( result.Nodes );
            }

            return new MatchResult<NodeT> ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="AlternatedMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( AlternatedMatcher AlternatedMatcher )
        {
            var result = new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, AlternatedMatcher, "Empty AlternatedMatcher" ) );
            foreach ( BaseMatcher matcher in AlternatedMatcher.PatternMatchers )
            {
                result = matcher.Accept ( this );
                if ( result.Success )
                    return result;
            }
            return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, AlternatedMatcher, "Failed to match any of the alternatives", result.Error ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="charMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( CharMatcher charMatcher )
        {
            if ( this.Reader.HasContent && this.Reader.IsNext ( charMatcher.Filter ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult<NodeT> ( new[] { charMatcher.StringFilter } );
            }
            return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, charMatcher, $"Expected '{StringUtilities.GetCharacterRepresentation ( charMatcher.Filter )}' but got '{( this.Reader.Peek ( ).HasValue ? StringUtilities.GetCharacterRepresentation ( this.Reader.Peek ( ).Value ) : "null" )}'" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="RangeMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( RangeMatcher RangeMatcher )
        {
            if ( this.Reader.HasContent && RangeMatcher.Range.ValueIn ( ( Char ) this.Reader.Peek ( ) ) )
            {
                return new MatchResult<NodeT> ( new[] { this.Reader.ReadString ( 1 ) } );
            }
            if ( this.Reader.HasContent )
            {
                var start = StringUtilities.GetCharacterRepresentation ( RangeMatcher.Range.Start );
                var end = StringUtilities.GetCharacterRepresentation ( RangeMatcher.Range.End  );
                return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, RangeMatcher, $"Expected character inside range ['{start}', '{end}'] but got '{StringUtilities.GetCharacterRepresentation ( this.Reader.Peek ( ) )}'" ) );
            }
            else
                return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, RangeMatcher, "Unexpected EOF" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="filterFuncMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( this.Reader.HasContent && filterFuncMatcher.Filter ( peek ) )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult<NodeT> ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, filterFuncMatcher, $"Character '{StringUtilities.GetCharacterRepresentation ( peek )}' did not pass the filter function {filterFuncMatcher.FullFilterName}" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="CharListMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( CharListMatcher CharListMatcher )
        {
            var peek = ( Char ) this.Reader.Peek ( );
            if ( this.Reader.HasContent && Array.IndexOf ( CharListMatcher.Whitelist, peek ) != -1 )
            {
                this.Reader.Advance ( 1 );
                return new MatchResult<NodeT> ( new[] { peek.ToString ( ) } );
            }
            return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, CharListMatcher, $"Expected character in whitelist ['{String.Join ( "', '", Array.ConvertAll ( CharListMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) )}'] but got '{StringUtilities.GetCharacterRepresentation ( peek )}'" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="rulePlaceholder"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( RulePlaceholder rulePlaceholder ) => this.RawRule ( rulePlaceholder.Name ).Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="stringMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( StringMatcher stringMatcher )
        {
            if ( this.Reader.HasContent && this.Reader.IsNext ( stringMatcher.StringFilter ) )
            {
                this.Reader.Advance ( stringMatcher.StringFilter.Length );
                return new MatchResult<NodeT> ( new[] { stringMatcher.StringFilter } );
            }
            return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, stringMatcher, $"Expected string '{StringUtilities.GetStringRepresentation ( stringMatcher.StringFilter )}' but got '{StringUtilities.GetStringRepresentation ( this.Reader.PeekString ( stringMatcher.StringFilter.Length ) ?? "null (not enough characters until EOF)" )}'" ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ignoreMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( IgnoreMatcher ignoreMatcher )
        {
            MatchResult<NodeT> res = ignoreMatcher.PatternMatcher.Accept ( this );
            return res.Success ? new MatchResult<NodeT> ( res.Nodes ) : res;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="joinMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( JoinMatcher joinMatcher )
        {
            MatchResult<NodeT> res = joinMatcher.PatternMatcher.Accept ( this );
            return res.Success
                ? new MatchResult<NodeT> ( res.Nodes, new[] { String.Join ( "", res.Strings ) } )
                : res;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="negatedMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var maxlen           = this.MaxLengthNegateds ? this.LengthCalculator.Calculate ( negatedMatcher ) : 1;
            MatchResult<NodeT> res      = negatedMatcher.PatternMatcher.Accept ( this );

            return res.Success
                ? new MatchResult<NodeT> ( new MatcherFailureException ( start, negatedMatcher, "Matched sequence that wasn't meant to be matched." ) )
                : new MatchResult<NodeT> ( new[] { this.Reader.ReadString ( maxlen ) } );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="optionalMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( OptionalMatcher optionalMatcher )
        {
            MatchResult<NodeT> res = optionalMatcher.PatternMatcher.Accept ( this );
            return res.Success ? res : new MatchResult<NodeT> ( Array.Empty<String> ( ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="repeatedMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            var nodeList = new List<NodeT> ( );
            var stringList = new List<String> ( );
            var mcount = 0;

            for ( mcount = 0; mcount < repeatedMatcher.Range.End; mcount++ )
            {
                MatchResult<NodeT> res = repeatedMatcher.PatternMatcher.Accept ( this );
                if ( !res.Success )
                    break;
                nodeList.AddRange ( res.Nodes );
                stringList.AddRange ( res.Strings );
            }

            return mcount < repeatedMatcher.Range.Start
                ? new MatchResult<NodeT> ( new MatcherFailureException ( start, repeatedMatcher, "Failed to match the pattern the minimum amount of times." ) )
                : new MatchResult<NodeT> ( nodeList.ToArray ( ), stringList.ToArray ( ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="ruleWrapper"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( RuleWrapper ruleWrapper )
        {
            this.RuleEnter ( ruleWrapper.Name );
            MatchResult<NodeT> res = ruleWrapper.PatternMatcher.Accept ( this );
            if ( res.Success && this.Factories.ContainsKey ( ruleWrapper.Name ) )
                res = new MatchResult<NodeT> ( new[] { this.Factory ( ruleWrapper.Name ) ( ruleWrapper.Name, res ) } );
            this.RuleMatch ( ruleWrapper.Name, res );
            this.RuleExit ( ruleWrapper.Name );
            return res;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="markerMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( MarkerMatcher markerMatcher )
        {
            MatchResult<NodeT> res = markerMatcher.PatternMatcher.Accept ( this );
            if ( res.Success )
            {
                var nodes = new NodeT[res.Nodes.Length + 1];
                Array.Copy ( res.Nodes, nodes, res.Nodes.Length );
                nodes[nodes.Length - 1] = this.MarkerNodeFactory ( "marker-node", res );
                res = new MatchResult<NodeT> ( nodes, res.Strings );
            }
            return res;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="eofMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( EOFMatcher eofMatcher ) => this.Reader.IsAtEOF
                ? new MatchResult<NodeT> ( Array.Empty<String> ( ) )
                : new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, eofMatcher, "Expected EOF." ) );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="savingMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( SavingMatcher savingMatcher )
        {
            MatchResult<NodeT> res = savingMatcher.PatternMatcher.Accept ( this );
            if ( res.Success )
                this.SaveMemory[savingMatcher.SaveName] = res.Strings[0];
            return res;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loadingMatcher"></param>
        /// <returns></returns>
        public MatchResult<NodeT> Visit ( LoadingMatcher loadingMatcher )
        {
            // Ensure content existence
            if ( !this.SaveMemory.ContainsKey ( loadingMatcher.SaveName ) )
                return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, loadingMatcher, $"Failed to load saved content from memory slot '{loadingMatcher.SaveName}'" ) );

            // Do normal StringMatcher logic
            var content = this.SaveMemory[loadingMatcher.SaveName];
            if ( !this.Reader.IsNext ( content ) )
                return new MatchResult<NodeT> ( new MatcherFailureException ( this.Reader.Location, loadingMatcher, $"Expected '{content}' but got '{this.Reader.PeekString ( content.Length ) ?? "EOF"}'." ) );

            // Return content on success
            this.Reader.Advance ( content.Length );
            return new MatchResult<NodeT> ( new[] { content } );
        }
    }
}
