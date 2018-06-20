using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GParse.Common.IO;
using GParse.Verbose.Exceptions;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Parsing
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
        /// char ::= ? \xFF, \b0101, \1321 and any other C escape
        /// code or normal char ? ;
        /// </summary>
        /// <returns></returns>
        private Char ParseChar ( )
        {
            if ( Consume ( '\\' ) )
            {
                String num;
                Char readChar;
                switch ( readChar = ( Char ) this.Reader.ReadChar ( ) )
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
        /// Parses all possible regex char classes
        /// </summary>
        /// <returns></returns>
        private BaseMatcher[] ParseCharacterClass ( )
        {
            if ( this.Consume ( "[:ascii:]" ) || this.Consume ( "\\p{ASCII}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( '\x00', '\xFF')
                };
            }
            else if ( this.Consume ( "[:alnum:]" ) || this.Consume ( "\\p{Alnum}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( 'A', 'Z'),
                    new CharRangeMatcher ( 'a', 'z'),
                    new CharRangeMatcher ( '0', '9')
                };
            }
            else if ( this.Consume ( "\\w" ) || this.Consume ( "[:word:]" ) )
            {
                return new BaseMatcher[]
                {
                    new CharRangeMatcher ( 'A', 'Z'),
                    new CharRangeMatcher ( 'a', 'z'),
                    new CharRangeMatcher ( '0', '9'),
                    new CharMatcher ( '_' )
                };
            }
            else if ( this.Consume ( "[:alpha:]" ) || this.Consume ( "\\p{Alpha}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( 'A', 'Z'),
                    new CharRangeMatcher ( 'a', 'z')
                };
            }
            else if ( this.Consume ( "[:blank:]" ) || this.Consume ( "\\p{Blank}" ) )
            {
                return new[]
                {
                    new MultiCharMatcher ( ' ', '\t' )
                };
            }
            else if ( this.Consume ( "[:cntrl:]" ) || this.Consume ( "\\p{Cntrl}" ) )
            {
                return new BaseMatcher[]
                {
                    new CharRangeMatcher ( '\x00', '\x1F'),
                    new CharMatcher ( '\x7F' ),
                };
            }
            else if ( this.Consume ( "\\d" ) || this.Consume ( "[:digit:]" ) || this.Consume ( "\\p{Digit}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( '0', '9'),
                };
            }
            else if ( this.Consume ( "[:graph:]" ) || this.Consume ( "\\p{Graph}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( '\x21', '\x7E'),
                };
            }
            else if ( this.Consume ( "[:lower:]" ) || this.Consume ( "\\p{Lower}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( 'a', 'z'),
                };
            }
            else if ( this.Consume ( "[:print:]" ) || this.Consume ( "\\p{Print}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( '\x20', '\x7E'),
                };
            }
            else if ( this.Consume ( "[:punct:]" ) || this.Consume ( "\\p{Punct}" ) )
            {
                return new[]
                {
                    new MultiCharMatcher ( '[', ']', '[', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',',
                        '.', '/', ':', ';', '<', '=', '>', '?', '@', '\\', '^', '_', '`', '{', '|', '}', '~', '-', ']' )
                };
            }
            else if ( this.Consume ( "\\s" ) || this.Consume ( "[:space:]" ) || this.Consume ( "\\p{Space}" ) )
            {
                return new[]
                {
                    new MultiCharMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ),
                };
            }
            else if ( this.Consume ( "[:upper:]" ) || this.Consume ( "\\p{Upper}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( 'A', 'Z'),
                };
            }
            else if ( this.Consume ( "[:xdigit:]" ) || this.Consume ( "\\p{XDigit}" ) )
            {
                return new[]
                {
                    new CharRangeMatcher ( 'A', 'F'),
                    new CharRangeMatcher ( 'a', 'f'),
                    new CharRangeMatcher ( '0', '9'),
                };
            }
            else
                return null;
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
                BaseMatcher[] matchers;
                if ( ( matchers = this.ParseCharacterClass ( ) ) != null )
                {
                    foreach ( BaseMatcher matcher in matchers )
                        opts.Add ( matcher );
                }
                else
                {
                    var ch = ParseChar ( );
                    // Actual ranges
                    if ( this.Reader.Peek ( 1 ) != ']' && this.Reader.Peek ( 1 ) != -1 && Consume ( '-' ) )
                        opts.Add ( new CharRangeMatcher ( ch, ParseChar ( ) ) );
                    else
                        opts.Add ( new CharMatcher ( ch ) );
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
            BaseMatcher val = this.ParseExpression ( true );
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
            return name == "EOF"
                ? ( BaseMatcher ) new EOFMatcher ( )
                : new RulePlaceholder ( name);
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
            else if ( Char.IsLetter ( ( Char ) this.Reader.Peek ( ) ) )
                return this.ParseRuleReference ( );
            else
            {
                BaseMatcher[] group = this.ParseCharacterClass ( );
                if ( group == null )
                    return null;
                return group.Length > 1 ? new AnyMatcher ( group ) : group[0];
            }
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
                return this.ParsePrefixedExpression ( ).Join ( );
            else if ( this.Consume ( "i:" ) || this.Consume ( "ignore:" ) )
                return this.ParsePrefixedExpression ( ).Ignore ( );
            else if ( this.Consume ( "m:" ) || this.Consume ( "mark:" ) )
                return this.ParsePrefixedExpression ( ).Mark ( );
            else if ( this.Consume ( "im:" ) || this.Consume ( "imark:" ) )
                return this.ParsePrefixedExpression ( ).Mark ( ).Ignore ( );
            else
                return this.ParseAtomic ( );
        }

        /// <summary>
        /// parses the {\d+[,[\d+]]} suffix
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        private BaseMatcher ParseRepeatSuffix ( BaseMatcher matcher )
        {
            var start = this.ParseInteger ( );
            var end = start + 1;
            if ( this.Consume ( ',' ) )
            {
                end = Int32.MaxValue;
                if ( Char.IsDigit ( ( Char ) this.Reader.Peek ( ) ) )
                    end = this.ParseInteger ( );
            }
            if ( start > end )
                throw new MatchExpressionException ( this.Reader.Location, "Range cannot have the start greater than the end." );
            return this.ParseSuffixedExpression ( matcher.Repeat ( start, end ) );
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
                matcher = this.ParseRepeatSuffix ( matcher );
                this.Expect ( '}' );
                return this.ParseSuffixedExpression ( matcher );
            }
            else if ( Consume ( '*' ) )
                return this.ParseSuffixedExpression ( matcher.Repeat ( 0, Int32.MaxValue ) );
            else if ( Consume ( '+' ) )
                return this.ParseSuffixedExpression ( matcher.Repeat ( 1, Int32.MaxValue ) );
            else if ( Consume ( '?' ) )
                return this.ParseSuffixedExpression ( matcher.Optional ( ) );
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
            BaseMatcher comp;
            var comps = new List<BaseMatcher> ( );
            this.ConsumeWhitespaces ( );
            while ( !this.Reader.EOF ( ) && ( comp = this.ParsePrefixedExpression ( ) ) != null )
            {
                comps.Add ( this.ParseSuffixedExpression ( comp ) );
                this.ConsumeWhitespaces ( );
            }
            return comps.Count > 1 ? new AllMatcher ( comps.ToArray ( ) ) : comps[0];
        }

        private BaseMatcher ParseExpression ( Boolean isGroup )
        {
            this.ConsumeWhitespaces ( );
            BaseMatcher matcher = this.ParseFixExpression ( );

            this.ConsumeWhitespaces ( );
            if ( this.Consume ( '|' ) )
                return matcher | this.ParseExpression ( isGroup );
            return matcher;
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
            BaseMatcher matcher = this.ParseExpression ( false );
            if ( !this.Reader.EOF ( ) )
                throw new MatchExpressionException ( this.Reader.Location, $"Expected EOF. (Text left: {this.Reader})" );
            this.Reader = null;
            return matcher;
        }
    }
}
