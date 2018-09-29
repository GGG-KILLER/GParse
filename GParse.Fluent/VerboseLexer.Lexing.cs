using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Common.Math;
using GParse.Common.Utilities;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Lexing;
using GParse.Fluent.Matchers;

namespace GParse.Fluent
{
    public abstract partial class FluentLexer<TokenTypeT> : IMatcherTreeVisitor<LexResult>
        where TokenTypeT : IEquatable<TokenTypeT>
    {
        private SourceCodeReader Reader;
        private readonly LexRulePredictor Predictor;
        internal readonly Dictionary<String, String> SaveMemory = new Dictionary<String, String> ( );

        public IEnumerable<Token<TokenTypeT>> Tokenize ( String input )
        {
            this.Reader = new SourceCodeReader ( input );
            while ( this.Reader.HasContent )
            {
                var matched = false;
                var peek = ( Char ) this.Reader.Peek ( );
                foreach ( var ruleName in this.Predictor.Suggest ( peek ) )
                {
                    if ( this.CompiledRules.ContainsKey ( ruleName ) )
                    {
                        SourceLocation start = this.Reader.Location;
                        Token<TokenTypeT> res = this.CompiledRules[ruleName] ( this.Reader, this );
                        if ( res != null )
                        {
                            matched = true;
                            this.Predictor.StoreResult ( peek, ruleName );
                            yield return res;
                            break;
                        }
                        this.Reader.Rewind ( start );
                    }
                    else
                    {
                        SourceLocation start = this.Reader.Location;
                        RuleDefinition<TokenTypeT> ruleDef = this.Rules[ruleName];
                        LexResult res = ruleDef.Body.Accept ( this );
                        if ( res.Success )
                        {
                            matched = true;
                            this.Predictor.StoreResult ( peek, ruleName );
                            yield return this.CreateToken ( ruleDef.Name, res.Match, ruleDef.Converter?.Invoke ( res.Match ) ?? res.Match,
                                ruleDef.Type, start.To ( this.Reader.Location ) );
                            break;
                        }
                        this.Reader.Rewind ( start );
                    }
                }
                if ( !matched )
                    throw new LexingException ( this.Reader.Location, "Couldn't match any rule." );
            }
        }

        /// <summary>
        /// The function which generates tokens
        /// </summary>
        /// <param name="name"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="sourceRange"></param>
        /// <returns></returns>
        protected virtual Token<TokenTypeT> CreateToken ( String name, String raw, Object value, TokenTypeT type, SourceRange sourceRange )
            => new Token<TokenTypeT> ( name, raw, value, type, sourceRange );

        private LexResult Unexpected ( Char expected, Char? received = null )
            => new LexResult ( new LexingException ( this.Reader.Location, $"Expected '{StringUtilities.GetCharacterRepresentation ( expected )}' but got '{StringUtilities.GetCharacterRepresentation ( received ?? ( Char ) this.Reader.Peek ( ) )}'" ) );

        private LexResult Unexpected ( String expected, String received = null )
            => new LexResult ( new LexingException ( this.Reader.Location, $"Expected '{StringUtilities.GetStringRepresentation ( expected )}' but got '{StringUtilities.GetStringRepresentation ( received ?? this.Reader.PeekString ( expected.Length ) )}'" ) );

        public LexResult Visit ( SequentialMatcher SequentialMatcher )
        {
            var strings = new String[SequentialMatcher.PatternMatchers.Length];
            for ( var i = 0; i < SequentialMatcher.PatternMatchers.Length; i++ )
            {
                BaseMatcher matcher = SequentialMatcher.PatternMatchers[i];
                SourceLocation start = this.Reader.Location;
                LexResult result = matcher.Accept ( this );
                if ( result.Success )
                    strings[i] = result.Match;
                else
                {
                    this.Reader.Rewind ( start );
                    return new LexResult ( new LexingException ( this.Reader.Location, "Failed to match entire construct.", result.Error ) );
                }
            }
            return new LexResult ( String.Join ( "", strings ) );
        }

        public LexResult Visit ( AlternatedMatcher AlternatedMatcher )
        {
            for ( var i = 0; i < AlternatedMatcher.PatternMatchers.Length; i++ )
            {
                BaseMatcher matcher = AlternatedMatcher.PatternMatchers[i];
                SourceLocation start = this.Reader.Location;
                LexResult result = matcher.Accept ( this );
                if ( result.Success )
                    return result;
                else
                    this.Reader.Rewind ( start );
            }
            return new LexResult ( new LexingException ( this.Reader.Location, "Unable to match any of the alternatives provided." ) );
        }

        public LexResult Visit ( CharMatcher charMatcher ) => this.Reader.HasContent && this.Reader.IsNext ( charMatcher.Filter )
                ? new LexResult ( this.Reader.ReadString ( 1 ) )
                : this.Unexpected ( charMatcher.Filter );

        public LexResult Visit ( RangeMatcher RangeMatcher )
        {
            Range<Char> range = RangeMatcher.Range;
            return this.Reader.HasContent && range.ValueIn ( this.Reader.Peek ( ).Value )
                ? new LexResult ( this.Reader.PeekString ( 1 ) )
                : new LexResult ( new LexingException ( this.Reader.Location, $"Expected char inside range [0x{range.Start:X}, 0x{range.End:X}] but got '{StringUtilities.GetCharacterRepresentation ( ( Char ) this.Reader.Peek ( ) )}'" ) );
        }

        public LexResult Visit ( EOFMatcher eofMatcher )
            => this.Reader.IsAtEOF
            ? new LexResult ( "" )
            : new LexResult ( new LexingException ( this.Reader.Location, "Expected EOF but got something else." ) );

        public LexResult Visit ( FilterFuncMatcher filterFuncMatcher ) => this.Reader.HasContent && filterFuncMatcher.Filter ( ( Char ) this.Reader.Peek ( ) )
                ? new LexResult ( this.Reader.ReadString ( 1 ) )
                : new LexResult ( new LexingException ( this.Reader.Location, $"Expected char that matched function '{filterFuncMatcher.FullFilterName}' but got '{StringUtilities.GetCharacterRepresentation ( ( Char ) this.Reader.Peek ( ) )}'" ) );

        public LexResult Visit ( IgnoreMatcher ignoreMatcher )
        {
            LexResult result = ignoreMatcher.PatternMatcher.Accept ( this );
            return result.Success ? new LexResult ( "" ) : result;
        }

        // This is unused here since all results are always joined
        // into a single string
        public LexResult Visit ( JoinMatcher joinMatcher )
            => joinMatcher.PatternMatcher.Accept ( this );

        public LexResult Visit ( MarkerMatcher markerMatcher )
            => markerMatcher.PatternMatcher.Accept ( this );

        public LexResult Visit ( CharListMatcher CharListMatcher )
        {
            var wlist = CharListMatcher.Whitelist;
            if ( this.Reader.HasContent )
            {
                var peek = ( Char ) this.Reader.Peek ( );
                for ( var i = 0; i < wlist.Length; i++ )
                    if ( peek == wlist[i] )
                        return new LexResult ( this.Reader.ReadString ( 1 ) );
            }
            return new LexResult ( new LexingException ( this.Reader.Location, $"Expected char inside whitelist ['{String.Join ( "', '", Array.ConvertAll ( wlist, StringUtilities.GetCharacterRepresentation ) )}'] but got '{StringUtilities.GetCharacterRepresentation ( ( Char ) this.Reader.Peek ( ) )}'" ) );
        }

        public LexResult Visit ( NegatedMatcher negatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            LexResult result = negatedMatcher.Accept ( this );
            this.Reader.Rewind ( start );

            return !result.Success
                ? new LexResult ( this.Reader.ReadString ( 1 ) )
                : new LexResult ( new LexingException ( start, "Matched segment that wasn't meant to be matched." ) );
        }

        public LexResult Visit ( OptionalMatcher optionalMatcher )
        {
            SourceLocation start = this.Reader.Location;
            LexResult result = optionalMatcher.Accept ( this );
            if ( !result.Success )
            {
                this.Reader.Rewind ( start );
                return new LexResult ( "" );
            }
            return result;
        }

        public LexResult Visit ( RepeatedMatcher repeatedMatcher )
        {
            SourceLocation start = this.Reader.Location;
            Range<UInt32> range = repeatedMatcher.Range;
            var list = new List<String> ( ( Int32 ) range.End );
            for ( var i = 0U; i <= range.End; i++ )
            {
                SourceLocation innerStart = this.Reader.Location;
                LexResult result =  repeatedMatcher.PatternMatcher.Accept ( this );
                if ( !result.Success )
                {
                    this.Reader.Rewind ( innerStart );
                    break;
                }
            }
            return list.Count < range.Start
                ? new LexResult ( new LexingException ( this.Reader.Location, "Couldn't match the patterns the minimum amount of times." ) )
                : new LexResult ( String.Join ( "", list ) );
        }

        public LexResult Visit ( RulePlaceholder rulePlaceholder )
        {
            if ( !this.Rules.ContainsKey ( rulePlaceholder.Name ) )
                throw new InvalidLexerException ( $"Undefined rule name {rulePlaceholder.Name} used in grammar." );
            return this.Rules[rulePlaceholder.Name].Body.Accept ( this );
        }

        public LexResult Visit ( RuleWrapper ruleWrapper )
        {
            if ( !this.Rules.ContainsKey ( ruleWrapper.Name ) )
                throw new InvalidLexerException ( $"Undefined rule name {ruleWrapper.Name} used in grammar." );
            return ruleWrapper.PatternMatcher.Accept ( this );
        }

        public LexResult Visit ( StringMatcher stringMatcher )
        {
            if ( this.Reader.HasContent && this.Reader.IsNext ( stringMatcher.StringFilter ) )
            {
                this.Reader.Advance ( stringMatcher.StringFilter.Length );
                return new LexResult ( stringMatcher.StringFilter );
            }
            return this.Unexpected ( stringMatcher.StringFilter );
        }

        public LexResult Visit ( SavingMatcher savingMatcher )
        {
            LexResult result = savingMatcher.PatternMatcher.Accept ( this );
            if ( result.Success )
            {
                this.SaveMemory[savingMatcher.SaveName] = result.Match;
                return result;
            }
            return result;
        }

        public LexResult Visit ( LoadingMatcher loadingMatcher ) => !this.SaveMemory.ContainsKey ( loadingMatcher.SaveName )
                ? new LexResult ( "" )
                : new LexResult ( this.SaveMemory[loadingMatcher.SaveName] );
    }
}
