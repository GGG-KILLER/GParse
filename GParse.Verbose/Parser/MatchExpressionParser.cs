using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Parser
{
    public class MatchExpressionParser
    {
        #region Utilities

        private void Expect ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                throw new MatchExpressionException ( this.Reader.Location, $"Expected '{ch}' but got '{( Char ) this.Reader.Peek ( )}'." );
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

        private static readonly IEnumerable<KeyValuePair<String, BaseMatcher[]>> SetLUT;
        private static readonly Func<SourceCodeReader,List<BaseMatcher>, Boolean> CompiledLUT;
        private readonly VerboseParser Parser;
        private SourceCodeReader Reader;

        public MatchExpressionParser ( VerboseParser parser )
        {
            this.Parser = parser;
        }

        /// <summary>
        /// Consume all whitespaces
        /// </summary>
        private void ConsumeWhitespaces ( )
        {
            while ( !this.Reader.EOF ( ) && Char.IsWhiteSpace ( ( Char ) this.Reader.Peek ( ) ) )
                this.Reader.Advance ( 1 );
        }

        /// <summary>
        /// char ::= ? \x, \b, \ and any other C escape code or
        /// normal char ? ;
        /// </summary>
        /// <returns></returns>
        private Char ParseChar ( )
        {
            if ( Consume ( '\\' ) )
            {
                String num;
                switch ( ( Char ) this.Reader.Peek ( ) )
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
                        return ( Char ) this.Reader.ReadChar ( );

                    case '0':
                        return '\0';

                    case 'a':
                        return '\a';

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
                        if ( this.Reader.IsNext ( '0' ) || this.Reader.IsNext ( '1' ) )
                        {
                            num = this.Reader.ReadStringWhile ( ch => ch == '0' || ch == '1' || ch == '_' )
                              .Replace ( "_", "" );
                            return ( Char ) Convert.ToInt32 ( num, 2 );
                        }
                        else
                            return '\b';

                    // Hex escapes
                    case 'x':
                        num = this.Reader.ReadStringWhile ( ch => ( '0' <= ch && ch <= '9' )
                            || ( 'a' <= ch && ch <= 'f' ) || ( 'A' <= ch && ch <= 'F' ) );
                        if ( num == "" )
                            throw new MatchExpressionException ( this.Reader.Location, "Invalid hexadecimal char escape code." );
                        return ( Char ) Convert.ToInt32 ( num, 16 );

                    // Decimal escapes
                    default:
                        num = this.Reader.ReadStringWhile ( Char.IsDigit );
                        if ( num == "" )
                            throw new MatchExpressionException ( this.Reader.Location, "Invalid decimal char escape code." );
                        return ( Char ) Convert.ToInt32 ( num, 10 );
                        #endregion Number-based character escape codes
                }
                throw new MatchExpressionException ( this.Reader.Location, "Invalid escape sequence." );
            }
            else if ( !this.Reader.EOF ( ) )
                return ( Char ) this.Reader.ReadChar ( );
            throw new MatchExpressionException ( this.Reader.Location, "Unfinished match expression. Expected a char but got EOF." );
        }

        /// <summary>
        /// digit ::= ? Char.IsDigit ? ; integer ::= { digit } ;
        /// </summary>
        /// <param name="radix"></param>
        /// <returns></returns>
        private Int32 ParseInteger ( Int32 radix = 10 )
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
                        return Convert.ToInt32 ( num, 2 );
                    }
                    catch ( Exception e )
                    {
                        throw new MatchExpressionException ( start, "Invalid binary number.", e );
                    }

                case 10:
                    num = this.Reader.ReadStringWhile ( Char.IsDigit );
                    try
                    {
                        return Convert.ToInt32 ( num, 10 );
                    }
                    catch ( Exception e )
                    {
                        throw new MatchExpressionException ( start, "Invalid decimal number.", e );
                    }

                case 16:
                    num = this.Reader.ReadStringWhile ( ch => Char.IsDigit ( ch ) || ( 'a' <= ch && ch <= 'f' ) || ( 'A' <= ch && ch <= 'F' ) );
                    try
                    {
                        return Convert.ToInt32 ( num, 16 );
                    }
                    catch ( Exception e )
                    {
                        throw new MatchExpressionException ( start, "Invalid hexadecimal number.", e );
                    }

                default:
                    throw new ArgumentException ( "Invalid number radix.", nameof ( radix ) );
            }
        }

        /// <summary>
        /// set-char-range ::= char, '-', char ; set ::= '[', {
        /// set-char-range | char }, ']' ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseSet ( )
        {
            var opts = new HashSet<BaseMatcher> ( );
            Expect ( '[' );
            while ( this.Reader.Peek ( ) != ']' && !this.Reader.EOF ( ) )
            {
                if ( this.Reader.IsNext ( "[:ascii:]" ) || this.Reader.IsNext ( "\\p{ASCII}" ) )
                    opts.Add ( Match.CharRange ( '\x00', '\xFF' ) );
                else if ( this.Reader.IsNext ( "[:alnum:]" ) || this.Reader.IsNext ( "\\p{Alnum}" ) )
                {
                    opts.Add ( Match.CharRange ( 'A', 'Z' ) );
                    opts.Add ( Match.CharRange ( 'a', 'z' ) );
                    opts.Add ( Match.CharRange ( '0', '9' ) );
                }
                else if ( this.Reader.IsNext ( "[:word:]" ) || this.Reader.IsNext ( "\\w" ) )
                {
                    opts.Add ( Match.CharRange ( 'A', 'Z' ) );
                    opts.Add ( Match.CharRange ( 'a', 'z' ) );
                    opts.Add ( Match.CharRange ( '0', '9' ) );
                    opts.Add ( Match.Char ( '_' ) );
                }
                else if ( this.Reader.IsNext ( "[:alpha:]" ) || this.Reader.IsNext ( "\\p{Alpha}" ) )
                {
                    opts.Add ( Match.CharRange ( 'A', 'Z' ) );
                    opts.Add ( Match.CharRange ( 'a', 'z' ) );
                }
                else if ( this.Reader.IsNext ( "[:blank:]" ) || this.Reader.IsNext ( "\\p{Blank}" ) )
                    opts.Add ( Match.Chars ( ' ', '\t' ) );
                else if ( this.Reader.IsNext ( "[:cntrl:]" ) || this.Reader.IsNext ( "\\p{Cntrl}" ) )
                {
                    opts.Add ( Match.CharRange ( '\x00', '\x1F' ) );
                    opts.Add ( Match.Char ( '\x7F' ) );
                }
                else if ( this.Reader.IsNext ( "\\d" ) || this.Reader.IsNext ( "[:digit:]" ) || this.Reader.IsNext ( "\\p{Digit}" ) )
                    opts.Add ( Match.CharRange ( '0', '9' ) );
                else if ( this.Reader.IsNext ( "[:graph:]" ) || this.Reader.IsNext ( "\\p{Graph}" ) )
                    opts.Add ( Match.CharRange ( '\x21', '\x7E' ) );
                else if ( this.Reader.IsNext ( "[:lower:]" ) || this.Reader.IsNext ( "\\p{Lower}" ) )
                    opts.Add ( Match.CharRange ( 'a', 'z' ) );
                else if ( this.Reader.IsNext ( "[:print:]" ) || this.Reader.IsNext ( "\\p{Print}" ) )
                    opts.Add ( Match.CharRange ( '\x20', '\x7E' ) );
                else
                {
                    var ch = ParseChar ( );
                    // Actual ranges
                    if ( this.Reader.Peek ( 1 ) != ']' && this.Reader.Peek ( 1 ) != -1 && Consume ( '-' ) )
                    {
                        var end = ParseChar ( );
                        opts.Add ( Match.CharRange ( ch, end ) );
                    }
                    else
                        opts.Add ( Match.Char ( ch ) );
                }
            }
            Expect ( ']' );

            return new AnyMatcher ( opts.ToArray ( ) );
        }

        /// <summary>
        /// group ::= '(', expression, ')' ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseGroup ( )
        {
            Expect ( '(' );
            BaseMatcher val = ParseSuffixExpression ( );
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
            return str.Length > 1 ? Match.String ( str ) : Match.Char ( str[0] );
        }

        /// <summary>
        /// rule-reference ::= ? Regex([a-zA-Z0-9_-]+) ? ;
        /// </summary>
        /// <returns></returns>
        private BaseMatcher ParseRuleReference ( )
        {
            var name = this.Reader.ReadStringWhile ( ch => Char.IsLetterOrDigit ( ch ) || ch == '-' || ch == '_' );
            return new RulePlaceholder ( name, this.Parser );
        }

        /// <summary>
        /// atomic ::= set | grouping | rule-reference | string ;
        /// </summary>
        /// <param name="throwOnFail">
        /// Whether null will be returned or an exception thrown
        /// when the parser fails to match an atomic-type element
        /// </param>
        /// <returns></returns>
        private BaseMatcher ParseAtomic ( Boolean throwOnFail = false )
        {
            if ( this.Reader.IsNext ( '[' ) )
                return this.ParseSet ( );
            else if ( this.Reader.IsNext ( '(' ) )
                return this.ParseGroup ( );
            else if ( this.Reader.IsNext ( '\'' ) || this.Reader.IsNext ( '"' ) )
                return this.ParseStringLiteral ( );
            else if ( Char.IsLetter ( ( Char ) this.Reader.Peek ( ) ) )
                return this.ParseRuleReference ( );
            else
                return !throwOnFail
                    ? ( BaseMatcher ) null
                    : throw new MatchExpressionException ( this.Reader.Location, "Invalid expression." );
        }

        /// <summary>
        /// repetition-suffix ::= '{', integer, [ ',', integer ],
        /// '}' ; expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private BaseMatcher ParseSuffix ( BaseMatcher matcher )
        {
            while ( true )
            {
                ConsumeWhitespaces ( );
                if ( Consume ( '{' ) )
                {
                    var start = Int32.Parse ( this.Reader.ReadStringWhile ( Char.IsDigit ) );
                    if ( Consume ( ',' ) )
                    {
                        if ( Char.IsDigit ( ( Char ) this.Reader.Peek ( ) ) )
                        {
                            var end = Int32.Parse ( this.Reader.ReadStringWhile ( Char.IsDigit ) );
                            if ( start > end )
                                throw new MatchExpressionException ( this.Reader.Location, "Range cannot have the start greater than the end." );
                            matcher = matcher.Repeat ( start, end );
                        }
                        else
                            matcher = matcher.Repeat ( start, Int32.MaxValue );
                    }
                    else
                        matcher = matcher.Repeat ( start, start + 1 );
                    Expect ( ')' );
                }
                else if ( Consume ( '*' ) )
                    matcher = matcher.Repeat ( 0, Int32.MaxValue );
                else if ( Consume ( '+' ) )
                    matcher = matcher.Repeat ( 1, Int32.MaxValue );
                else if ( Consume ( '?' ) )
                    matcher = matcher.Optional ( );
                else
                    break;
            }
            return matcher;
        }

        /// <summary>
        /// expression ::= { atomic, { '*' | '+' |
        /// suffix-repetition } };
        /// </summary>
        /// <param name="isGroup">
        /// whether this is a subcall (grouping)
        /// </param>
        /// <returns></returns>
        private BaseMatcher ParseSuffixExpression ( Boolean isGroup = true )
        {
            BaseMatcher comp;
            var comps = new List<BaseMatcher> ( );
            while ( ( comp = ParseAtomic ( !isGroup ) ) != null )
                comps.Add ( ParseSuffix ( comp ) );
            return comps.Count > 1 ? new AllMatcher ( comps.ToArray ( ) ) : comps[0];
        }

        /// <summary>
        /// Parses a string into a series of matchers according to
        /// the language.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public BaseMatcher Parse ( String expression )
        {
            this.Reader = new SourceCodeReader ( expression );
            BaseMatcher matcher = this.ParseSuffixExpression ( false );
            do
            {
                this.ConsumeWhitespaces ( );
                if ( this.Reader.IsNext ( '|' ) )
                    matcher = matcher | this.ParseSuffixExpression ( false );
                else
                    break;
            }
            while ( true );
            if ( !this.Reader.EOF ( ) )
                throw new MatchExpressionException ( this.Reader.Location, $"Expected EOF. (Text left: {this.Reader})" );
            this.Reader = null;
            return matcher;
        }
    }
}
