﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GParse.Common.IO;
using GParse.Common.Math;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Utilities;

namespace GParse.Fluent.Parsing
{
    public class ExpressionParser
    {
        #region Utilities

        private void Expect ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                throw new InvalidExpressionException ( this.Reader.Location, $"Expected '{ch}' but got '{( Char ) this.Reader.Peek ( )}'." );
            this.Reader.Advance ( 1 );
        }

        private Boolean Consume ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                return false;
            this.Reader.Advance ( 1 );
            return true;
        }

        private Boolean Consume ( String str )
        {
            if ( !this.Reader.IsNext ( str ) )
                return false;
            this.Reader.Advance ( str.Length );
            return true;
        }

        #endregion Utilities

        #region Hardcoded things

        internal static readonly IReadOnlyDictionary<String, BaseMatcher> RegexClassesLUT = new Dictionary<String, BaseMatcher>
        {
            { @"[:ascii:]", new RangeMatcher ( '\x00', '\xFF' ) },
            { @"\p{ASCII}", new RangeMatcher ( '\x00', '\xFF' ) },
            { @"[:alnum:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"\p{Alnum}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"[:word:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) },
            { @"\w", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) },
            { @"\W", new NegatedMatcher ( new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) ) },
            { @"[:alpha:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' )
            ) },
            { @"\p{Alpha}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' )
            ) },
            { @"[:blank:]", new CharListMatcher ( ' ', '\t' ) },
            { @"\p{Blank}", new CharListMatcher ( ' ', '\t' ) },
            // TODO: Word boundaries
            { @"[:cntrl:]", new AlternatedMatcher (
                new RangeMatcher ( '\x00', '\x1F' ),
                new CharMatcher ( '\x7F' )
            ) },
            { @"\p{Cntrl}", new AlternatedMatcher (
                new RangeMatcher ( '\x00', '\x1F' ),
                new CharMatcher ( '\x7F' )
            ) },
            { @"[:digit:]", new RangeMatcher ( '0', '9' ) },
            { @"\p{Digit}", new RangeMatcher ( '0', '9' ) },
            { @"\d",        new RangeMatcher ( '0', '9' ) },
            { @"\D",        new NegatedMatcher ( new RangeMatcher ( '0', '9' ) ) },
            { @"[:graph:]", new RangeMatcher ( '\x21', '\x7E' ) },
            { @"\p{Graph}", new RangeMatcher ( '\x21', '\x7E' ) },
            { @"[:lower:]", new RangeMatcher ( 'a', 'z' ) },
            { @"\p{Lower}", new RangeMatcher ( 'a', 'z' ) },
            { @"[:print:]", new RangeMatcher ( '\x20', '\x7E' ) },
            { @"\p{Print}", new RangeMatcher ( '\x20', '\x7E' ) },
            { @"[:punct:]", new CharListMatcher ( '[', ']', '[', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',',
                '.', '/', ':', ';', '<', '=', '>', '?', '@', '\\', '^', '_', '`', '{', '|', '}', '~', '-', ']' ) },
            { @"\p{Punct}", new CharListMatcher ( '[', ']', '[', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',',
                '.', '/', ':', ';', '<', '=', '>', '?', '@', '\\', '^', '_', '`', '{', '|', '}', '~', '-', ']' ) },
            { @"[:space:]", new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"\p{Space}", new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"\s",        new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"[:upper:]", new RangeMatcher ( 'A', 'Z' ) },
            { @"\p{Upper}", new RangeMatcher ( 'A', 'Z' ) },
            { @"[:xdigit:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'F' ),
                new RangeMatcher ( 'a', 'f' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"\p{XDigit}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'F' ),
                new RangeMatcher ( 'a', 'f' ),
                new RangeMatcher ( '0', '9' )
            ) },
        };

        #endregion Hardcoded things

        private enum MatchModifier
        {
            None,
            Ignore,
            Join
        }

        private SourceCodeReader Reader;
        private readonly Stack<MatchModifier> ModifierStack = new Stack<MatchModifier> ( );

        /// <summary>
        /// Rule reference lookup table for memoization
        /// </summary>
        private readonly IDictionary<String, BaseMatcher> RuleLUT = new Dictionary<String, BaseMatcher>
        {
            { "EOF", new EOFMatcher ( ) }
        };

        /// <summary>
        /// Consume all whitespaces
        /// </summary>
        private void ConsumeWhitespaces ( )
        {
            this.Reader.ReadStringWhile ( Char.IsWhiteSpace );
        }

        /// <summary>
        /// char ::= ? \xFF, \b0101, \1321 and any other C escape
        /// code or normal char ? ;
        /// </summary>
        /// <returns></returns>
        private Char ParseChar ( )
        {
            if ( Consume ( '\\' ) )
            {
                Char readChar = this.Reader.Read ( ) ?? throw new InvalidExpressionException ( this.Reader.Location, "Unfinished escape." );
                switch ( readChar )
                {
                    #region Basic Character Escapes

                    case '"':
                    case '\'':
                    case ']':
                    case '*':
                    case '+':
                    case '?':
                    case '\\':
                    case '>':
                        return readChar;

                    case '0':
                        return '\0';

                    case 'a':
                        return '\a';

                    // b should be here but we also have binary escapes.

                    case 'f':
                        return '\f';

                    case 'n':
                        return '\n';

                    case 'r':
                        return '\r';

                    case 't':
                        return '\t';

                    case 'v':
                        return '\v';

                    #endregion Basic Character Escapes

                    #region Number-based character escape codes

                    // Binary escapes
                    case 'b':
                        return this.Reader.IsNext ( '0' ) || this.Reader.IsNext ( '1' )
                            ? ( Char ) this.ParseInteger ( 2 )
                            : '\b';

                    // Octal escapes
                    case 'o':
                        return ( Char ) this.ParseInteger ( 8 );

                    // Decimal escapes
                    default:
                        return ( Char ) this.ParseInteger ( 10 );

                    // Hex escapes
                    case 'x':
                        return ( Char ) this.ParseInteger ( 16 );

                        #endregion Number-based character escape codes
                }
                throw new InvalidExpressionException ( this.Reader.Location, "Invalid escape sequence." );
            }
            else if ( this.Reader.HasContent )
                return this.Reader.Read ( ).Value;
            throw new InvalidExpressionException ( this.Reader.Location, "Unfinished match expression. Expected a char but got EOF." );
        }

        /// <summary>
        /// digit ::= ? Char.IsDigit ? ; integer ::= { digit } ;
        /// </summary>
        /// <param name="radix"></param>
        /// <returns></returns>
        private UInt32 ParseInteger ( Int32 radix = 10 )
        {
            String num;
            Common.SourceLocation start = this.Reader.Location;
            switch ( radix )
            {
                case 2:
                    num = this.Reader.ReadStringWhile ( ch => ch == '0' || ch == '1' || ch == '_' )
                        .Replace ( "_", "" );
                    try
                    {
                        return Convert.ToUInt32 ( num, 2 );
                    }
                    catch ( Exception e )
                    {
                        throw new InvalidExpressionException ( start, "Invalid binary number.", e );
                    }

                case 10:
                    num = this.Reader.ReadStringWhile ( Char.IsDigit )
                        .Replace ( "_", "" );
                    try
                    {
                        return Convert.ToUInt32 ( num, 10 );
                    }
                    catch ( Exception e )
                    {
                        throw new InvalidExpressionException ( start, "Invalid decimal number.", e );
                    }

                case 16:
                    num = this.Reader.ReadStringWhile ( ch => Char.IsDigit ( ch ) || ( 'a' <= ch && ch <= 'f' ) || ( 'A' <= ch && ch <= 'F' ) )
                        .Replace ( "_", "" );
                    try
                    {
                        return Convert.ToUInt32 ( num, 16 );
                    }
                    catch ( Exception e )
                    {
                        throw new InvalidExpressionException ( start, "Invalid hexadecimal number.", e );
                    }

                default:
                    throw new ArgumentException ( "Invalid number radix.", nameof ( radix ) );
            }
        }

        /// <summary>
        /// Parses all possible regex char classes
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private Boolean TryParseCharacterClass ( out BaseMatcher matcher )
        {
            matcher = null;
            if ( !( this.Reader.IsNext ( '[' ) || this.Reader.IsNext ( '\\' ) ) )
                return false;
            foreach ( KeyValuePair<String, BaseMatcher> kv in RegexClassesLUT )
            {
                if ( this.Consume ( kv.Key ) )
                {
                    matcher = kv.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// set-char-range ::= char, '-', char ; set ::= '[', {
        /// set-char-range | char }, ']' ;
        /// </summary>
        /// <returns></returns>
        private AlternatedMatcher ParseSet ( )
        {
            // Use a hashset so we don't get repeated matchers
            var opts = new NoDuplicatesList<BaseMatcher> ( );
            Expect ( '[' );

            // First character in a regex set can be a ] which
            // should be interpreted as normal char
            if ( this.Consume ( ']' ) )
                opts.Add ( new CharMatcher ( ']' ) );

            // Read the contents of the set
            while ( this.Reader.Peek ( ) != ']' && this.Reader.HasContent )
            {
                if ( this.TryParseCharacterClass ( out BaseMatcher matcher ) )
                {
                    // Don't nest alternated matchers unnecessarily
                    if ( matcher is AlternatedMatcher alternated )
                        opts.AddRange ( alternated.PatternMatchers );
                    else
                        opts.Add ( matcher );
                }
                else
                {
                    var ch = ParseChar ( );
                    // Actual ranges
                    if ( this.Reader.Peek ( 1 ) != ']' && Consume ( '-' ) )
                        opts.Add ( new RangeMatcher ( ch, ParseChar ( ) ) );
                    else
                        opts.Add ( new CharMatcher ( ch ) );
                }
            }
            Expect ( ']' );

            return new AlternatedMatcher ( opts.ToArray ( ) );
        }

        /// <summary>
        /// group ::= '(', expression, ')' ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseGroup ( )
        {
            // Literally a wrapper for an expression
            Expect ( '(' );
            BaseMatcher val = this.ParseExpression ( );
            Expect ( ')' );
            return val;
        }

        /// <summary>
        /// string ::= { char } ; string-literal ::= '\'', string,
        /// '\'' ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseStringLiteral ( )
        {
            // Define the string delimiter
            var delim = this.Reader.IsNext ( '\'' ) ? '\'' : '"';

            // Parse the string contents
            Expect ( delim );
            var buff = new StringBuilder ( );
            while ( !this.Reader.IsNext ( delim ) )
                buff.Append ( ParseChar ( ) );
            Expect ( delim );

            // Build the string and use the appropriate matcher type
            var str = buff.ToString ( );
            return str.Length > 1 ? ( BaseMatcher ) new StringMatcher ( str ) : new CharMatcher ( str[0] );
        }

        /// <summary>
        /// rule-reference ::= ? Regex([a-zA-Z0-9_-]+) ? ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseRuleReference ( )
        {
            var name = this.Reader.ReadStringWhile ( ch => Char.IsLetterOrDigit ( ch ) || ch == '-' || ch == '_' );

            if ( !this.RuleLUT.ContainsKey ( name ) )
                this.RuleLUT[name] = new RulePlaceholder ( name );
            return this.RuleLUT[name];
        }

        /// <summary>
        /// atomic ::= set | grouping | rule-reference | string ;
        /// </summary>
        private BaseMatcher ParseAtomic ( )
        /// <returns></returns>
        {
            if ( this.Reader.IsNext ( '[' ) )
                return this.ParseSet ( );
            else if ( this.Reader.IsNext ( '(' ) )
                return this.ParseGroup ( );
            else if ( this.Reader.IsNext ( '\'' ) || this.Reader.IsNext ( '"' ) )
                return this.ParseStringLiteral ( );
            else if ( this.Consume ( "l:" ) || this.Consume ( "load:" ) )
            {
                var name = this.Reader.ReadStringWhile ( ch => Char.IsLetterOrDigit ( ch ) || ch == '-' || ch == '_' );
                if ( String.IsNullOrWhiteSpace ( name ) )
                    throw new InvalidExpressionException ( this.Reader.Location, "Invalid save name." );
                return new LoadingMatcher ( name );
            }
            else if ( Char.IsLetter ( ( Char ) this.Reader.Peek ( ) ) )
                return this.ParseRuleReference ( );
            else if ( this.TryParseCharacterClass ( out BaseMatcher matcher ) )
                return matcher;

            return null;
        }

        /// <summary>
        /// Parses the possible prefixes of an expression
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParsePrefixedExpression ( )
        {
            this.ConsumeWhitespaces ( );
            if ( this.Consume ( '!' ) || this.Consume ( '-' ) )
                return this.ParsePrefixedExpression ( ).Negate ( );
            else if ( this.Consume ( "j:" ) || this.Consume ( "join:" ) )
            {
                // Don't keep nested joins/ignores as they'll be useless
                if ( this.ModifierStack.Contains ( MatchModifier.Join ) || this.ModifierStack.Contains ( MatchModifier.Ignore ) )
                    return this.ParsePrefixedExpression ( );

                this.ModifierStack.Push ( MatchModifier.Join );
                BaseMatcher matcher = this.ParsePrefixedExpression ( );
                this.ModifierStack.Pop ( );
                return matcher.Join ( );
            }
            else if ( this.Consume ( "i:" ) || this.Consume ( "ignore:" ) )
            {
                if ( this.ModifierStack.Contains ( MatchModifier.Ignore ) )
                    return this.ParsePrefixedExpression ( );

                this.ModifierStack.Push ( MatchModifier.Ignore );
                BaseMatcher matcher = this.ParsePrefixedExpression ( );
                this.ModifierStack.Pop ( );
                return matcher.Ignore ( );
            }
            else if ( this.Consume ( "m:" ) || this.Consume ( "mark:" ) )
                return this.ParsePrefixedExpression ( ).Mark ( );
            else if ( this.Consume ( "im:" ) || this.Consume ( "imark:" ) )
            {
                BaseMatcher matcher = this.ParsePrefixedExpression ( ).Mark ( );
                // Don't ignore already ignored contents
                if ( !this.ModifierStack.Contains ( MatchModifier.Ignore ) )
                    matcher = matcher.Ignore ( );
                return matcher;
            }
            else if ( this.Consume ( "s:" ) || this.Consume ( "save:" ) )
            {
                var name = this.Reader.ReadStringWhile ( ch => Char.IsLetterOrDigit ( ch ) || ch == '-' || ch == '_' );
                if ( String.IsNullOrWhiteSpace ( name ) )
                    throw new InvalidExpressionException ( this.Reader.Location, "Invalid save name." );
                this.Expect ( ':' );
                return new SavingMatcher ( name, this.ParsePrefixedExpression ( ) );
            }
            else
                return this.ParseAtomic ( );
        }

        // Multiply two unsigned 32-bit integers taking into
        // acount overflow by clamping value to UInt32.MaxValue
        private static UInt32 RangeIntMul ( UInt32 lhs, UInt32 rhs )
        {
            // Don't even try on ±inf
            if ( lhs == UInt32.MaxValue || rhs == UInt32.MaxValue )
                return UInt32.MaxValue;

            // Return inf on overflow
            var mul = unchecked ( lhs * rhs );
            return mul % lhs != 0 || mul % rhs != 0 ? UInt32.MaxValue : mul;
        }

        /// <summary>
        /// Creates a <see cref="RepeatedMatcher" /> with specific
        /// optimizations applied
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static RepeatedMatcher RepeatMatcher ( BaseMatcher matcher, UInt32 start, UInt32 end )
        {
            // Optimization
            if ( matcher is RepeatedMatcher repeated )
            {
                /* expr{α}{β} ≡ expr{α·β} */
                if ( repeated.Range.IsSingle && start == end )
                    return new RepeatedMatcher ( repeated.PatternMatcher, new Range<UInt32> ( RangeIntMul ( repeated.Range.Start, start ) ) );

                /* expr{a, b}{c, d} ≡ expr{a·c, b·d} IF b·c ≥ a·(c + 1) - 1 */
                if ( repeated.Range.End * start >= repeated.Range.Start * ( start + 1 ) - 1 )
                    return new RepeatedMatcher ( repeated.PatternMatcher,
                        // "safe" multiplication in case of inifinities
                        new Range<UInt32> ( RangeIntMul ( repeated.Range.Start, start ), RangeIntMul ( repeated.Range.End, end ) ) );
            }

            // expr{start, end}
            return new RepeatedMatcher ( matcher, new Range<UInt32> ( start, end ) );
        }

        /// <summary>
        /// parses the {\d+[,[\d+]]} suffix
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private RepeatedMatcher ParseRepeatSuffix ( BaseMatcher matcher )
        {
            var start = this.ParseInteger ( );
            var end = start;

            this.ConsumeWhitespaces ( );
            if ( this.Consume ( ',' ) )
            {
                this.ConsumeWhitespaces ( );
                end = UInt32.MaxValue;
                if ( Char.IsDigit ( ( Char ) this.Reader.Peek ( ) ) )
                    end = this.ParseInteger ( );
                this.ConsumeWhitespaces ( );
            }
            if ( start > end )
                throw new InvalidExpressionException ( this.Reader.Location, "Range cannot have the start greater than the end." );

            return RepeatMatcher ( matcher, start, end );
        }

        /// <summary>
        /// repetition-suffix ::= '{', integer, [ ',', integer ],
        /// '}' ; expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private BaseMatcher ParseSuffixedExpression ( BaseMatcher matcher )
        {
            this.ConsumeWhitespaces ( );
            if ( this.Consume ( '{' ) )
            {
                this.ConsumeWhitespaces ( );
                matcher = this.ParseRepeatSuffix ( matcher );
                this.Expect ( '}' );
                return this.ParseSuffixedExpression ( matcher );
            }
            else if ( Consume ( '*' ) )
                return this.ParseSuffixedExpression ( RepeatMatcher ( matcher, 0, UInt32.MaxValue ) );
            else if ( Consume ( '+' ) )
                return this.ParseSuffixedExpression ( RepeatMatcher ( matcher, 1, UInt32.MaxValue ) );
            else if ( Consume ( '?' ) )
            {
                /* expr{α, β}? ≡ expr{0, β} IF α ≤ 1 */
                if ( matcher is RepeatedMatcher repeated && repeated.Range.Start <= 1 )
                    return RepeatMatcher ( repeated, 0, repeated.Range.End );

                return this.ParseSuffixedExpression ( matcher.Optional ( ) );
            }
            else
                return matcher;
        }

        /// <summary>
        /// expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseFixExpression ( )
        {
            var components = new List<BaseMatcher> ( );
            var acc = new StringBuilder ( );

            void SubmitBuffer ( )
            {
                // Submit stringified contents
                if ( acc.Length > 0 )
                {
                    // Submit the proper matcher so we don't waste
                    // resources when executing
                    components.Add ( acc.Length == 1
                        ? ( BaseMatcher ) new CharMatcher ( acc[0] )
                        : new StringMatcher ( acc.ToString ( ) ) );
                    acc.Clear ( );
                }
            }

            // Submit an item to the components list
            void AddItem ( BaseMatcher matcher )
            {
                /* 'a' 'bc' ≡ 'abc' */
                if ( matcher is IStringContainerMatcher strContMatcher )
                    acc.Append ( strContMatcher.StringFilter );
                else
                {
                    SubmitBuffer ( );

                    /* expr₁ (expr₂ expr₃) ≡ expr₁ expr₂ expr₃ */
                    if ( matcher is SequentialMatcher sequential )
                        foreach ( BaseMatcher subMatcher in sequential.PatternMatchers )
                            AddItem ( subMatcher );
                    else
                        components.Add ( matcher );
                }
            }

            this.ConsumeWhitespaces ( );
            while ( this.Reader.HasContent )
            {
                var prefixedExpression = this.ParsePrefixedExpression ( );
                this.ConsumeWhitespaces ( );
                if ( prefixedExpression == null )
                    break;
                AddItem ( this.ParseSuffixedExpression ( prefixedExpression ) );
                this.ConsumeWhitespaces ( );
            }

            SubmitBuffer ( );
            return components.Count > 1
                ? new SequentialMatcher ( components.ToArray ( ) )
                : components.Count > 0
                    ? components[0]
                    : throw new InvalidOperationException ( "Idk wtf happened." );
        }

        private BaseMatcher ParseExpression ( )
        {
            this.ConsumeWhitespaces ( );
            BaseMatcher lhs = this.ParseFixExpression ( );

            this.ConsumeWhitespaces ( );
            if ( this.Consume ( '|' ) )
            {
                BaseMatcher rhs = this.ParseExpression ( );
                var exprs = new NoDuplicatesList<BaseMatcher> ( );

                // Merge lhs if it's a nested alternated matcher
                if ( lhs is AlternatedMatcher lhsAlternated )
                    exprs.AddRange ( lhsAlternated.PatternMatchers );
                else
                    exprs.Add ( lhs );
                // Merge rhs if it's a nested alternated matcher
                if ( rhs is AlternatedMatcher rhsAlternated )
                    exprs.AddRange ( rhsAlternated.PatternMatchers );
                else
                    exprs.Add ( rhs );

                // Return unique sequence of expressions
                lhs = new AlternatedMatcher ( exprs.ToArray ( ) );
            }
            return lhs;
        }

        /// <summary>
        /// Parses a string into a series of matchers according to
        /// the language.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public BaseMatcher Parse ( String expression )
        {
            if ( String.IsNullOrWhiteSpace ( expression ) )
                throw new InvalidExpressionException ( Common.SourceLocation.Zero, "Expression cannot be empty or whitespace." );
            this.Reader = new SourceCodeReader ( expression );
            this.ModifierStack.Clear ( );

            this.ModifierStack.Push ( MatchModifier.None );
            BaseMatcher matcher = this.ParseExpression ( );
            Debug.Assert ( this.ModifierStack.Pop ( ) == MatchModifier.None, "Unhandled modifier stack value." );

            // Make sure the modifiers' stack is clear
            Debug.Assert ( this.ModifierStack.Count == 0, "Modifiers' stack isn't empty.", "Stack contents: {0}",
                String.Join ( ", ", this.ModifierStack ) );
            if ( this.Reader.HasContent )
                throw new InvalidExpressionException ( this.Reader.Location, $@"Expected EOF. (Text left: {this.Reader})
Reader internal state: ({this.Reader.ContentLeftSize}, {this.Reader.Location})" );
            this.Reader = null;
            return matcher;
        }
    }
}