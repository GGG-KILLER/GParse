using System;
using System.Collections.Generic;
using GParse.Common.IO;
using GParse.Common.Math;
using GParse.Common.Utilities;
using GParse.Parsing.Lexing.Modules.Regex.AST;

namespace GParse.Parsing.Lexing.Modules.Regex
{
    /// <summary>
    /// I'm too lazy to adhere to any standard so this regex
    /// parser is pretty much #yolo. Binary, octal and hexadecimal
    /// escapes are only limited by <see cref="Char.MaxValue" />.
    /// </summary>
    internal class RegexParser
    {
        internal static readonly RegexClassTree ClassTree;

        static RegexParser ( )
        {
            ClassTree = new RegexClassTree ( );
            ClassTree.AddClass ( @"[:ascii:]", new Range ( new Range<Char> ( '\x00', '\xFF' ) ) );
            ClassTree.AddClass ( @"\p{ASCII}", new Range ( new Range<Char> ( '\x00', '\xFF' ) ) );
            ClassTree.AddClass ( @"[:alnum:]", new Alternation ( new[]{
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) ),
                new Range ( new Range<Char> ( '0', '9' ) )
            }, false ) );
            ClassTree.AddClass ( @"\p{Alnum}", new Alternation ( new[]{
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) ),
                new Range ( new Range<Char> ( '0', '9' ) )
            }, false ) );
            ClassTree.AddClass ( @"[:word:]", new Alternation ( new Node[]
            {
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) ),
                new Range ( new Range<Char> ( '0', '9' ) ),
                new Literal ( '_' )
            }, false ) );
            ClassTree.AddClass ( @"\w", new Alternation ( new Node[]
            {
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) ),
                new Range ( new Range<Char> ( '0', '9' ) ),
                new Literal ( '_' )
            }, false ) );
            ClassTree.AddClass ( @"\W", new Alternation ( new Node[]
            {
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) ),
                new Range ( new Range<Char> ( '0', '9' ) ),
                new Literal ( '_' )
            }, true ) );
            ClassTree.AddClass ( @"[:alpha:]", new Alternation ( new[]
            {
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) )
            }, false ) );
            ClassTree.AddClass ( @"\p{Alpha}", new Alternation ( new[]
            {
                new Range ( new Range<Char> ( 'A', 'Z' ) ),
                new Range ( new Range<Char> ( 'a', 'z' ) )
            }, false ) );
            ClassTree.AddClass ( @"[:blank:]", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' )
            }, false ) );
            ClassTree.AddClass ( @"\p{Blank}", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' )
            }, false ) );
            // TODO: Word boundaries
            ClassTree.AddClass ( @"[:cntrl:]", new Alternation ( new Node[]
            {
                new Range ( new Range<Char> ( '\x00', '\x1F' ) ),
                new Literal ( '\x7F' )
            }, false ) );
            ClassTree.AddClass ( @"\p{Cntrl}", new Alternation ( new Node[]
            {
                new Range ( new Range<Char> ( '\x00', '\x1F' ) ),
                new Literal ( '\x7F' )
            }, false ) );
            ClassTree.AddClass ( @"[:digit:]", new Range ( new Range<Char> ( '0', '9' ) ) );
            ClassTree.AddClass ( @"\p{Digit}", new Range ( new Range<Char> ( '0', '9' ) ) );
            ClassTree.AddClass ( @"\d", new Range ( new Range<Char> ( '0', '9' ) ) );
            ClassTree.AddClass ( @"\D", new Alternation ( new[]
            {
                new Range ( new Range<Char> ( '0', '9' ) )
            }, true ) );
            ClassTree.AddClass ( @"[:graph:]", new Range ( new Range<Char> ( '\x21', '\x7E' ) ) );
            ClassTree.AddClass ( @"\p{Graph}", new Range ( new Range<Char> ( '\x21', '\x7E' ) ) );
            ClassTree.AddClass ( @"[:lower:]", new Range ( new Range<Char> ( 'a', 'z' ) ) );
            ClassTree.AddClass ( @"\p{Lower}", new Range ( new Range<Char> ( 'a', 'z' ) ) );
            ClassTree.AddClass ( @"[:print:]", new Range ( new Range<Char> ( '\x20', '\x7E' ) ) );
            ClassTree.AddClass ( @"\p{Print}", new Range ( new Range<Char> ( '\x20', '\x7E' ) ) );
            ClassTree.AddClass ( @"[:punct:]", new Alternation ( new[]
            {
                new Literal ( '[' ),
                new Literal ( ']' ),
                new Literal ( '[' ),
                new Literal ( '!' ),
                new Literal ( '\"' ),
                new Literal ( '#' ),
                new Literal ( '$' ),
                new Literal ( '%' ),
                new Literal ( '&' ),
                new Literal ( '\'' ),
                new Literal ( '(' ),
                new Literal ( ')' ),
                new Literal ( '*' ),
                new Literal ( '+' ),
                new Literal ( ',' ),
                new Literal ( '.' ),
                new Literal ( '/' ),
                new Literal ( ':' ),
                new Literal ( ';' ),
                new Literal ( '<' ),
                new Literal ( '=' ),
                new Literal ( '>' ),
                new Literal ( '?' ),
                new Literal ( '@' ),
                new Literal ( '\\' ),
                new Literal ( '^' ),
                new Literal ( '_' ),
                new Literal ( '`' ),
                new Literal ( '{' ),
                new Literal ( '|' ),
                new Literal ( '}' ),
                new Literal ( '~' ),
                new Literal ( '-' ),
                new Literal ( ']' )
            }, false ) );
            ClassTree.AddClass ( @"\p{Punct}", new Alternation ( new[]
            {
                new Literal ( '[' ),
                new Literal ( ']' ),
                new Literal ( '[' ),
                new Literal ( '!' ),
                new Literal ( '\"' ),
                new Literal ( '#' ),
                new Literal ( '$' ),
                new Literal ( '%' ),
                new Literal ( '&' ),
                new Literal ( '\'' ),
                new Literal ( '(' ),
                new Literal ( ')' ),
                new Literal ( '*' ),
                new Literal ( '+' ),
                new Literal ( ',' ),
                new Literal ( '.' ),
                new Literal ( '/' ),
                new Literal ( ':' ),
                new Literal ( ';' ),
                new Literal ( '<' ),
                new Literal ( '=' ),
                new Literal ( '>' ),
                new Literal ( '?' ),
                new Literal ( '@' ),
                new Literal ( '\\' ),
                new Literal ( '^' ),
                new Literal ( '_' ),
                new Literal ( '`' ),
                new Literal ( '{' ),
                new Literal ( '|' ),
                new Literal ( '}' ),
                new Literal ( '~' ),
                new Literal ( '-' ),
                new Literal ( ']' )
            }, false ) );
            ClassTree.AddClass ( @"[:space:]", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' ),
                new Literal ( '\n' ),
                new Literal ( '\r' ),
                new Literal ( '\v' ),
                new Literal ( '\f' )
            }, false ) );
            ClassTree.AddClass ( @"\p{Space}", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' ),
                new Literal ( '\n' ),
                new Literal ( '\r' ),
                new Literal ( '\v' ),
                new Literal ( '\f' )
            }, false ) );
            ClassTree.AddClass ( @"\s", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' ),
                new Literal ( '\n' ),
                new Literal ( '\r' ),
                new Literal ( '\v' ),
                new Literal ( '\f' )
            }, false ) );
            ClassTree.AddClass ( @"\S", new Alternation ( new[]
            {
                new Literal ( ' ' ),
                new Literal ( '\t' ),
                new Literal ( '\n' ),
                new Literal ( '\r' ),
                new Literal ( '\v' ),
                new Literal ( '\f' )
            }, true ) );
            ClassTree.AddClass ( @"[:upper:]", new Range ( new Range<Char> ( 'A', 'Z' ) ) );
            ClassTree.AddClass ( @"\p{Upper}", new Range ( new Range<Char> ( 'A', 'Z' ) ) );
            ClassTree.AddClass ( @"[:xdigit:]", new Alternation ( new[]
            {
                new Range ( new Range<Char> ( 'A', 'F' ) ),
                new Range ( new Range<Char> ( 'a', 'f' ) ),
                new Range ( new Range<Char> ( '0', '9' ) )
            }, false ) );
            ClassTree.AddClass ( @"\p{XDigit}", new Alternation ( new[]
            {
                new Range ( new Range<Char> ( 'A', 'F' ) ),
                new Range ( new Range<Char> ( 'a', 'f' ) ),
                new Range ( new Range<Char> ( '0', '9' ) )
            }, false ) );
        }

        #region Utilities

        private void Expect ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                throw new InvalidRegexException ( this.Reader.Location, $"Expected '{ch}' but got '{this.Reader.Peek ( )?.ToString ( ) ?? "EOF"}'." );
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

        private readonly SourceCodeReader Reader;
        private Int32 CaptureGroupCount;
        private Int32 GroupDepth;

        public RegexParser ( String expression )
        {
            if ( String.IsNullOrEmpty ( expression ) )
                throw new InvalidRegexException ( Common.SourceLocation.Zero, "Expression cannot be null or empty." );
            this.Reader = new SourceCodeReader ( expression );
        }

        /// <summary>
        /// Consume all whitespaces
        /// </summary>
        private void ConsumeWhitespaces ( )
        {
            var wscount = this.Reader.FindOffset ( Char.IsWhiteSpace );
            if ( wscount > 0 )
                this.Reader.Advance ( wscount );
        }

        private Node ParseLiteral ( Boolean insideRange )
        {
            if ( !this.Reader.HasContent )
                return null;

            Common.SourceLocation start = this.Reader.Location;
            // Don't re-do stuff, character class is already
            // checked in ranges at this point in execution
            if ( !insideRange && this.TryParseCharacterClass ( out Node node ) )
                return node;
            else if ( this.Consume ( '\\' ) )
            {
                var ch = this.Reader.Read ( ) ?? throw new InvalidRegexException ( start, "Unfinished escape code." );

                switch ( ch )
                {
                    #region Basic Character Escapes

                    case '"':
                    case '\'':
                    case '[':
                    case ']':
                    case '*':
                    case '+':
                    case '?':
                    case '\\':
                    case '>':
                    case '-':
                    case '|':
                    case '.':
                        return new Literal ( ch );

                    case '0':
                        return new Literal ( '\0' );

                    case 'a':
                        return new Literal ( '\a' );

                    // b should be here but we also have binary escapes.

                    case 'f':
                        return new Literal ( '\f' );

                    case 'n':
                        return new Literal ( '\n' );

                    case 'r':
                        return new Literal ( '\r' );

                    case 't':
                        return new Literal ( '\t' );

                    case 'v':
                        return new Literal ( '\v' );

                    #endregion Basic Character Escapes
                    #region Number-based character escape codes

                    // Binary escapes
                    case 'b':
                        return new Literal ( this.Reader.IsNext ( '0' ) || this.Reader.IsNext ( '1' ) ? ( Char ) this.ParseInteger ( 2 ) : '\b' );

                    // Octal escapes
                    case 'o':
                        return new Literal ( ( Char ) this.ParseInteger ( 8 ) );

                    // Hex escapes
                    case 'x':
                        return new Literal ( ( Char ) this.ParseInteger ( 16 ) );

                    // Capture references
                    case Char c when Char.IsDigit ( c ):
                        var i = ( UInt32 ) c - '0';

                        // If we still have digits left to read,
                        // then do so.
                        if ( this.Reader.HasContent && Char.IsDigit ( ( Char ) this.Reader.Peek ( ) ) )
                            i += this.ParseInteger ( 10 ) * 10;

                        return insideRange
                            ? ( Node ) new Literal ( ( Char ) i )
                            : this.CaptureGroupCount >= i
                                ? new CaptureReference ( ( Int32 ) i )
                                : throw new InvalidRegexException ( start, $"Invalid capture group referenced ({i}/{this.CaptureGroupCount})." );

                    #endregion Number-based character escape codes

                    default:
                        throw new InvalidRegexException ( start, "Invalid escape." );
                }
            }
            else if ( this.Reader.HasContent )
            {
                var ch = ( Char ) this.Reader.Read ( );
                switch ( ch )
                {
                    case '+':
                    case '*':
                    case '?':
                        throw new InvalidRegexException ( start, "Unexpected modifier." );

                    case '.':
                        return new Range ( new Range<Char> ( Char.MinValue, Char.MaxValue ) );

                    default:
                        return new Literal ( ch );
                }
            }
            else return null;
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
                        throw new InvalidRegexException ( start, "Invalid binary number.", e );
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
                        throw new InvalidRegexException ( start, "Invalid decimal number.", e );
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
                        throw new InvalidRegexException ( start, "Invalid hexadecimal number.", e );
                    }

                default:
                    throw new ArgumentException ( "Invalid number radix.", nameof ( radix ) );
            }
        }

        /// <summary>
        /// Parses all possible regex char classes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Boolean TryParseCharacterClass ( out Node node )
        {
            if ( this.Reader.IsNext ( '[' ) || this.Reader.IsNext ( '\\' ) )
            {
                (var className, Node classNode) = ClassTree.FindClass ( this.Reader );
                if ( className == null || classNode == null )
                {
                    node = null;
                    return false;
                }

                this.Reader.Advance ( className.Length );
                node = classNode;
                return true;
            }

            node = null;
            return false;
        }

        /// <summary>
        /// set-char-range ::= char, '-', char ; set ::= '[', {
        /// set-char-range | char }, ']' ;
        /// </summary>
        /// <returns></returns>
        private Node ParseSet ( )
        {
            // Use a hashset so we don't get repeated matchers
            var opts = new NoDuplicatesList<Node> ( );
            Common.SourceLocation start = this.Reader.Location;

            this.Expect ( '[' );

            // Check for negation of the alternation
            var neg = this.Consume ( '^' );

            // First character in a regex set can be a ] which
            // should be interpreted as normal char
            if ( this.Consume ( ']' ) )
                opts.Add ( new Literal ( ']' ) );

            // Read the contents of the set
            while ( this.Reader.Peek ( ) != ']' && this.Reader.HasContent )
            {
                if ( this.TryParseCharacterClass ( out Node matcher ) )
                {
                    /* OPTIMIZATION: Don't nest alternated matchers unnecessarily */
                    if ( matcher is Alternation alternated && alternated.IsNegated == neg )
                        opts.AddRange ( alternated.Children );
                    else
                        opts.Add ( matcher );
                }
                else
                {
                    Common.SourceLocation startLoc = this.Reader.Location;
                    var startLiteral = this.ParseLiteral ( true ) as Literal;
                    // Actual ranges
                    if ( this.Reader.Peek ( 1 ) != ']' && this.Consume ( '-' ) )
                    {
                        var endLiteral = this.ParseLiteral ( true ) as Literal;
                        if ( endLiteral.Value < startLiteral.Value )
                            throw new InvalidRegexException ( startLoc, "Range cannot have start greater than end." );

                        // Check for intersections
                        var range = new Range ( new Range<Char> ( startLiteral.Value, endLiteral.Value ) );
                        if ( opts.Find ( node => node is Range tmp && range.CharRange.IntersectsWith ( tmp.CharRange ) ) is Range range2 )
                        {
                            opts.Remove ( range2 );
                            opts.Add ( new Range ( new Range<Char> (
                                ( Char ) Math.Min ( range.CharRange.Start, range2.CharRange.Start ),
                                ( Char ) Math.Max ( range.CharRange.End, range2.CharRange.End )
                            ) ) );
                        }

                        opts.Add ( range );
                    }
                    else
                        opts.Add ( startLiteral );
                }
            }
            this.Expect ( ']' );

            return neg || opts.Count > 1
                ? new Alternation ( opts, neg )
                : opts.Count > 0
                    ? opts[0]
                    : throw new InvalidRegexException ( start, "Cannot have an empty set." );
        }

        /// <summary>
        /// group ::= '(', [ '?:' ], expression, ')' ;
        /// </summary>
        /// <returns></returns>
        private Node ParseGroup ( )
        {
            // Literally a wrapper for an expression
            this.Expect ( '(' );

            var capture = !this.Consume ( "?:" );
            this.GroupDepth++;
            Node inner = this.ParseExpression ( );
            this.GroupDepth--;

            this.Expect ( ')' );

            return capture ? new Capture ( ++this.CaptureGroupCount, inner ) : inner;
        }

        /// <summary>
        /// atomic ::= set | grouping | rule-reference | string ;
        /// </summary>
        private Node ParseAtomic ( )
        /// <returns></returns>
        {
            if ( this.Reader.IsNext ( '[' ) )
                return this.ParseSet ( );
            else if ( this.Reader.IsNext ( '(' ) )
                return this.ParseGroup ( );
            else
                return this.ParseLiteral ( false );
        }

        // Multiply two unsigned 32-bit integers taking into
        // acount overflow by clamping value to UInt32.MaxValue
        private static UInt32 SaturatingMultiplication ( in UInt32 lhs, in UInt32 rhs ) => SaturatingMath.Multiply ( lhs, rhs, UInt32.MinValue, UInt32.MaxValue );

        /// <summary>
        /// Creates a <see cref="Repetition" /> with specific
        /// optimizations applied
        /// </summary>
        /// <param name="node"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static Repetition RepeatMatcher ( in Node node, in UInt32 start, in UInt32 end )
        {
            // Optimization
            if ( node is Repetition repetition )
            {
                /* OPTIMIZATION: expr{α}{β} ≡ expr{α·β} */
                if ( repetition.Range.IsSingle && start == end )
                    return new Repetition ( new Range<UInt32> ( SaturatingMultiplication ( repetition.Range.Start, start ) ), repetition.Inner );

                /* OPTIMIZATION: expr{a, b}{c, d} ≡ expr{a·c, b·d} ↔ b·c ≥ a·(c + 1) - 1 */
                if ( repetition.Range.End * start >= repetition.Range.Start * ( start + 1 ) - 1 )
                    return new Repetition (
                        // "safe" multiplication in case of inifinities
                        new Range<UInt32> ( SaturatingMultiplication ( repetition.Range.Start, start ), SaturatingMultiplication ( repetition.Range.End, end ) ), repetition.Inner );
            }

            // expr{start, end}
            return new Repetition ( new Range<UInt32> ( start, end ), node );
        }

        /// <summary>
        /// parses the {\d+[,[\d+]]} suffix
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Repetition ParseRepeatExpressionSuffix ( Node node )
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

            if ( start == 0 && end == 0 )
                throw new InvalidRegexException ( this.Reader.Location, "Cannot have a repetition of count 0." );
            if ( start > end )
                throw new InvalidRegexException ( this.Reader.Location, "Range cannot have the start greater than the end." );

            return RepeatMatcher ( node, start, end );
        }

        /// <summary>
        /// repetition-suffix ::= '{', integer, [ ',', integer ],
        /// '}' ; expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Node ParseSuffixedExpression ( Node node )
        {
            Common.SourceLocation start = this.Reader.Location;
            if ( this.Consume ( '{' ) )
            {
                this.ConsumeWhitespaces ( );
                node = this.ParseRepeatExpressionSuffix ( node );
                this.Expect ( '}' );
                return this.ParseSuffixedExpression ( node );
            }
            else if ( this.Consume ( '*' ) )
                return this.ParseSuffixedExpression ( RepeatMatcher ( node, 0, UInt32.MaxValue ) );
            else if ( this.Consume ( '+' ) )
                return this.ParseSuffixedExpression ( RepeatMatcher ( node, 1, UInt32.MaxValue ) );
            else if ( this.Consume ( '?' ) )
            {
                /* OPTIMIZATION: expr{α, β}? ≡ expr{0, β} ↔ α ≤ 1 ∧ β ≠ ∞ */
                if ( node is Repetition repetition
                    // Don't do this for expr* and expr+, since
                    // this has another meaning with them
                    && repetition.Range.Start <= 1 && repetition.Range.End != UInt32.MaxValue )
                    return this.ParseSuffixedExpression ( RepeatMatcher ( repetition.Inner, 0, repetition.Range.End ) );

                node.IsLazy = true;
                return this.ParseSuffixedExpression ( node );
            }
            else
                return node;
        }

        /// <summary>
        /// expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <returns></returns>
        private Node ParseSequence ( )
        {
            var components = new List<Node> ( );

            // Submit an item to the components list
            void AddItem ( Node matcher )
            {
                /* OPTIMIZATION: expr₁ (expr₂ expr₃) ≡ expr₁ expr₂ expr₃ */
                if ( matcher is Sequence sequential && !sequential.IsLazy )
                    foreach ( Node subMatcher in sequential.Children )
                        AddItem ( subMatcher );
                else
                    components.Add ( matcher );
            }

            Common.SourceLocation start = this.Reader.Location;

            while ( this.Reader.HasContent )
            {
                Node atomic = this.ParseAtomic ( );
                if ( atomic == null )
                    break;

                AddItem ( this.ParseSuffixedExpression ( atomic ) );
            }

            /* OPTIMIZATION: seq{exprA} ≡ exprA */
            return components.Count > 1
                ? new Sequence ( components )
                : components.Count > 0
                    ? components[0]
                    : throw new InvalidRegexException ( start, "How the hell did you mange to create an empty sequence?" );
        }

        private Node ParseExpression ( )
        {
            Node lhs = this.ParseSequence ( );

            if ( this.Consume ( '|' ) )
            {
                Node rhs = this.ParseExpression ( );

                var exprs = new NoDuplicatesList<Node> ( );

                /* OPTIMIZATION: [exprSetA]|[exprSetB]   ≡ [exprSetA∪exprSetB] */
                /* OPTIMIZATION: [^exprSetA]|[^exprSetB] ≡ [^exprSetA∪exprSetB] */
                if ( lhs is Alternation la
                    && rhs is Alternation ra
                    && la.IsNegated == ra.IsNegated )
                {
                    exprs.AddRange ( la.Children );
                    exprs.AddRange ( ra.Children );
                    lhs = new Alternation ( exprs, la.IsNegated );
                }
                else
                {
                    /* OPTIMIZATION: [exprSetA]|exprB ≡ [exprSetAexprB] */
                    if ( lhs is Alternation lhsAlternation && !lhsAlternation.IsNegated )
                        exprs.AddRange ( lhsAlternation.Children );
                    else
                        exprs.Add ( lhs );

                    /* OPTIMIZATION: exprA|[exprSetB] ≡ [exprAexprSetB] */
                    if ( rhs is Alternation rhsAlternation && !rhsAlternation.IsNegated )
                        exprs.AddRange ( rhsAlternation.Children );
                    else
                        exprs.Add ( rhs );

                    lhs = exprs.Count > 1
                        ? new Alternation ( exprs, false )
                        /* OPTIMIZATION: seq{expr} ≡ expr */
                        : exprs.Count > 0
                            ? exprs[0]
                            : throw new InvalidRegexException ( this.Reader.Location, "How did you even manage to create an empty alternation?" );
                }
            }
            return lhs;
        }

        /// <summary>
        /// Parses a string into a series of matchers according to
        /// the language.
        /// </summary>
        /// <returns></returns>
        public Node Parse ( )
        {
            Node matcher = this.ParseExpression ( );
            if ( this.Reader.HasContent )
                throw new InvalidRegexException ( this.Reader.Location, "Expected EOF." );

            return matcher;
        }
    }
}
